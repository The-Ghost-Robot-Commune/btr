using System;
using System.Collections.Generic;
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
		public RemoteDispatcherProxy(IDispatcher localDispatcher)
		{
			this.LocalDispatcher = localDispatcher;

			LocalDispatcher.RegisterListenerForAll(this);
			LocalDispatcher.AddToForwardIgnoreList(this);
		}

		public IDispatcher LocalDispatcher { get; private set; }

		public void HandleMessage(IContext sender, IMessage message)
		{
			// All messages should be forwarded to the remote dispatcher
			throw new NotImplementedException();
		}
	}
}
