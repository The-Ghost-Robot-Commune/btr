using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	/// <summary>
	/// A class that connects to another dispatcher in another process/AppDomain and forwadrs messages between that one and the local one
	/// </summary>
	public class RemoteDispatcherProxy : IListener
	{
		

		private int messageBufferIndex;
		private readonly List<IMessage>[] messageBuffer;
		private volatile bool messageWaiting;
		private readonly object messageBufferLock;

		public RemoteDispatcherProxy(IDispatcher localDispatcher, ISerializer serializer, IRemoteCommunicator remoteCommunicator)
		{
			this.LocalDispatcher = localDispatcher;
			this.Serializer = serializer;
			this.RemoteCommunicator = remoteCommunicator;


			messageBuffer = new List<IMessage>[2];
			messageBuffer[0] = new List<IMessage>();
			messageBuffer[1] = new List<IMessage>();
			messageBufferIndex = 0;

			messageWaiting = false;
			messageBufferLock = new object();

			LocalDispatcher.RegisterListenerForAll(this);
			LocalDispatcher.AddToForwardIgnoreList(this);
			RemoteCommunicator.RegisterReceiver(ReceiveRemoteMessage);
		}

		public IDispatcher LocalDispatcher { get; private set; }

		public ISerializer Serializer { get; private set; }

		public IRemoteCommunicator RemoteCommunicator { get; private set; }

		public void HandleMessage(IContext sender, IMessage message)
		{
			// All messages are forwarded to the remote dispatcher
			MemoryStream stream = new MemoryStream();
			Serializer.Serialize(message, stream);
			RemoteCommunicator.Send(stream);
		}

		/// <summary>
		/// Forwards all the (if any) messages from the remote dispatcher to the local one.
		/// </summary>
		public void ForwardRemoteMessages()
		{
			// Avoid to do thread locking as far as possible
			if (messageWaiting)
			{
				List<IMessage> messages;
				lock (messageBufferLock)
				{
					messages = messageBuffer[messageBufferIndex];
					messageBufferIndex = (messageBufferIndex == 0 ? 1 : 0);
					messageWaiting = false;
				}


				LocalDispatcher.ForwardSend(messages);
			}
		}

		private void ReceiveRemoteMessage(MemoryStream stream)
		{
			IMessage message = Serializer.Deserialize(stream);

			lock (messageBufferLock)
			{
				messageBuffer[messageBufferIndex].Add(message);
			}

			// Notify the other thread that there is at least one message waiting
			messageWaiting = true;
		}
	}
}
