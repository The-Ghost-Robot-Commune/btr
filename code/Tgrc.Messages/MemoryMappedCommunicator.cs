using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tgrc.Thread;

namespace Tgrc.Messages
{
	public class MemoryMappedCommunicator : IRemoteCommunicator
	{
		/// <summary>
		/// The total amount of bytes to allocate per buffer.
		/// </summary>
		public static readonly int MemoryCapacity = 4 * 1024 * 1024;
		private const int StreamBufferCount = 2;
		private const int ReceiveTimeoutMs = 500;

		private bool disposedValue;
		private volatile bool run;
		private volatile bool receiverThreadDone;

		private readonly MemoryMappedFile sendFile;
		private readonly MemoryMappedViewStream[] sendStreams;
		private int sendCurrentIndex;

		private readonly IThread receiverThread;
		private readonly MemoryMappedFile receiveFile;
		private readonly MemoryMappedViewStream[] receiveStreams;
		private int receiveCurrentIndex;
		private int totalReceiveCount;
		private readonly MemoryStream receiverLocalMemory;

		private readonly Sync sendSync;
		private readonly Sync receiveSync;

		private readonly List<Action<MemoryStream>> receiverDelegates;

		public MemoryMappedCommunicator(IThreadStarter threadStarter, string mappingBaseName, bool isHost)
		{
			this.MappingBaseName = mappingBaseName;
			this.IsHost = isHost;
			MappingNameSend = MappingBaseName + ".HostSend";
			MappingNameReceive = MappingBaseName + ".HostReceive";
			if (!IsHost)
			{
				var temp = MappingNameSend;
				MappingNameSend = MappingNameReceive;
				MappingNameReceive = temp;
			}
			disposedValue = false;
			run = true;
			receiverThreadDone = false;

			long totalMappedMemory = MemoryCapacity * StreamBufferCount;
			sendFile = MemoryMappedFile.CreateOrOpen(MappingNameSend, totalMappedMemory, MemoryMappedFileAccess.ReadWrite);
			receiveFile = MemoryMappedFile.CreateOrOpen(MappingNameReceive, totalMappedMemory, MemoryMappedFileAccess.ReadWrite);
			sendStreams = new MemoryMappedViewStream[StreamBufferCount];
			receiveStreams = new MemoryMappedViewStream[StreamBufferCount];

			long memoryPerBuffer = MemoryCapacity;
			for (int i = 0; i < StreamBufferCount; i++)
			{
				sendStreams[i] = sendFile.CreateViewStream(i * memoryPerBuffer, memoryPerBuffer, MemoryMappedFileAccess.ReadWrite);
				receiveStreams[i] = receiveFile.CreateViewStream(i * memoryPerBuffer, memoryPerBuffer, MemoryMappedFileAccess.ReadWrite);
			}
			sendCurrentIndex = 0;
			receiveCurrentIndex = 0;
			TotalSendCount = 0;
			totalReceiveCount = 0;

			receiverLocalMemory = new MemoryStream(new byte[MemoryCapacity], 0, MemoryCapacity, true, true);

			sendSync = new Sync(MappingNameSend);
			receiveSync = new Sync(MappingNameReceive);

			receiverDelegates = new List<Action<MemoryStream>>();

			Thread.ThreadStart startInfo = new Thread.ThreadStart(ReceiveThread, this);
			receiverThread = threadStarter.CreateThread(startInfo);
		}

		public bool IsHost { get; private set; }
		public string MappingBaseName { get; private set; }
		public string MappingNameSend { get; private set; }
		public string MappingNameReceive { get; private set; }
		public string MappingNameSync { get { return MappingBaseName + ".Sync"; } }

		public int TotalSendCount { get; private set; }
		public int TotalReceiveCount { get { return totalReceiveCount; } }
		public bool ReceiverThreadRunning { get { return receiverThread.IsRunning; } }

		public void StartThreads()
		{
			receiverThread.Start();
		}

		public void RegisterReceiver(Action<MemoryStream> receiver)
		{
			if (ReceiverThreadRunning)
			{
				throw new InvalidOperationException("Can't register receivers after the thread have been started.");
			}
			receiverDelegates.Add(receiver);
		}

		public void Send(MemoryStream data)
		{
			// TODO Verify that the specified data fits in the buffer

			// If we are not using a timeout we get the side effect that both the "host" and "client" will suspend 
			// and wait if the other one, for some reason, can't keep up. The drawback to this is that if one side crashes/hangs the other will freeze.
			sendSync.Empty.WaitOne();

			// No mutex lock needed as we use different buffers/views for each batch of data. So no synchronization beyound the semaphores are needed
			// Get the current buffer/stream that we should write to
			var stream = sendStreams[sendCurrentIndex];
			stream.Position = 0;
			// Adjust the buffer index for the next use
			++sendCurrentIndex;
			if (sendCurrentIndex >= StreamBufferCount)
			{
				sendCurrentIndex = 0;
			}

			try
			{
				data.Position = 0;
				data.CopyTo(stream);
			}
			catch (Exception e)
			{
				// Errors here is most likely because we are trying to send more data than the buffer can hold
				throw;
			}

			// Signal to the receiving side that we have added data to the buffer
			sendSync.Full.Release();
			++TotalSendCount;
		}

		private static void ReceiveThread(object parameter)
		{
			MemoryMappedCommunicator _this = (MemoryMappedCommunicator)parameter;
			while (_this.run)
			{
				_this.Receive();
			}

			_this.receiverThreadDone = true;
		}

		private void Receive()
		{
			// Wait for some content in the buffer
			if (!receiveSync.Full.WaitOne(ReceiveTimeoutMs))
			{
				// Timeout reached. Return so we can check if we should shutdown
				return;
			}

			var stream = receiveStreams[receiveCurrentIndex];
			stream.Position = 0;
			// Adjust the buffer index for the next use
			++receiveCurrentIndex;
			if (receiveCurrentIndex >= StreamBufferCount)
			{
				receiveCurrentIndex = 0;
			}

			try
			{
				receiverLocalMemory.Position = 0;
				stream.CopyTo(receiverLocalMemory);
			}
			catch (Exception e)
			{
				// Unknown what kind of errors are likely to show up here
				throw;
			}

			// Signal that there is one more open space in the buffer
			receiveSync.Empty.Release();
			Interlocked.Increment(ref totalReceiveCount);

			foreach (var r in receiverDelegates)
			{
				try
				{
					receiverLocalMemory.Position = 0;
					r(receiverLocalMemory);
				}
				catch (Exception e)
				{
					// TODO handle this
				}
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// Signal to the receiver thread that we are about to shut down
					run = false;

					// Wait on the receiver thread to terminate. 
					while (!receiverThreadDone)
					{
						// This is an intentional lock of the current thread 
					}

					// All threads have terminated at this point, it is now safe to dispose of all synchronization primitives
					receiveSync.Dispose();
					sendSync.Dispose();

					receiverLocalMemory.Dispose();

					for (int i = StreamBufferCount - 1; i >= 0; i--)
					{
						receiveStreams[i].Dispose();
						sendStreams[i].Dispose();
					}
					receiveFile.Dispose();
					sendFile.Dispose();
				}

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		private struct Sync : IDisposable
		{
			public readonly Semaphore Full;
			public readonly Semaphore Empty;

			public Sync(string baseName)
			{
				this.Full = new Semaphore(0, StreamBufferCount, baseName + ".Full");
				this.Empty = new Semaphore(StreamBufferCount, StreamBufferCount, baseName + ".Empty");
			}

			public void Dispose()
			{
				Empty.Dispose();
				Full.Dispose();
			}
		}
	}
}
