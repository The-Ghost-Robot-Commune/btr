using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	public class MemoryMappedCommunicator : IRemoteCommunicator
	{
		/// <summary>
		/// The total amount of bytes to allocate for the memory mapping. Will be evenly split between the different buffers
		/// </summary>
		public static readonly long MemoryCapacity = 4 * 1024 * 1024;
		private const int StreamBufferCount = 2;

		private bool disposedValue;

		private readonly MemoryMappedFile sendFile;
		private readonly MemoryMappedViewStream[] sendStreams;
		private int sendCurrentIndex;

		private readonly MemoryMappedFile receiveFile;
		private readonly MemoryMappedViewStream[] receiveStreams;
		private int receiveCurrentIndex;
		private int totalReceiveCount;

		private readonly Sync sendSync;
		private readonly Sync receiveSync;

		public MemoryMappedCommunicator(string mappingBaseName, bool isHost)
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

			sendFile = MemoryMappedFile.CreateOrOpen(MappingNameSend, MemoryCapacity, MemoryMappedFileAccess.ReadWrite);
			receiveFile = MemoryMappedFile.CreateOrOpen(MappingNameReceive, MemoryCapacity, MemoryMappedFileAccess.ReadWrite);
			sendStreams = new MemoryMappedViewStream[StreamBufferCount];
			receiveStreams = new MemoryMappedViewStream[StreamBufferCount];

			long memoryPerBuffer = MemoryCapacity / StreamBufferCount;
			for (int i = 0; i < StreamBufferCount; i++)
			{
				sendStreams[i] = sendFile.CreateViewStream(i * memoryPerBuffer, memoryPerBuffer, MemoryMappedFileAccess.ReadWrite);
				receiveStreams[i] = receiveFile.CreateViewStream(i * memoryPerBuffer, memoryPerBuffer, MemoryMappedFileAccess.ReadWrite);
			}
			sendCurrentIndex = 0;
			receiveCurrentIndex = 0;
			TotalSendCount = 0;
			totalReceiveCount = 0;

			sendSync = new Sync(MappingNameSend);
			receiveSync = new Sync(MappingNameReceive);
		}

		public bool IsHost { get; private set; }
		public string MappingBaseName { get; private set; }
		public string MappingNameSend { get; private set; }
		public string MappingNameReceive { get; private set; }
		public string MappingNameSync { get { return MappingBaseName + ".Sync"; } }

		public int TotalSendCount { get; private set; }
		public int TotalReceiveCount { get { return totalReceiveCount; } }

		public void RegisterReceiver(Action<byte[]> receiver)
		{
			
		}

		public void Send(byte[] data)
		{
			sendSync.Empty.WaitOne();

			// Get the current buffer/stream that we should write to
			var stream = sendStreams[sendCurrentIndex];
			// Adjust the buffer index for the next use
			++sendCurrentIndex;
			if (sendCurrentIndex >= StreamBufferCount)
			{
				sendCurrentIndex = 0;
			}

			stream.Write(data, 0, data.Length);

			// Signal to the receiving side that we have added data to the buffer
			sendSync.Full.Release();


			++TotalSendCount;
		}

		private void Receive()
		{
			// Wait for some content in the buffer
			receiveSync.Full.WaitOne();



			// Signal that there is one more open space in the buffer
			receiveSync.Empty.Release();


			Interlocked.Increment(ref totalReceiveCount);
		}


		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					sendStreams[1].Dispose();
					sendStreams[0].Dispose();
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

		private struct Sync
		{
			public readonly Semaphore Full;
			public readonly Semaphore Empty;
			//public readonly Mutex Mutex;

			public Sync(string baseName)
			{
				this.Full = new Semaphore(0, StreamBufferCount, baseName + ".Full");
				this.Empty = new Semaphore(StreamBufferCount, StreamBufferCount, baseName + ".Empty");
				//this.Mutex = new Mutex(false, baseName + ".Mutex");
			}

		}
	}
}
