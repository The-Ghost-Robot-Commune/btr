using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tgrc.Messages;

namespace Tgrc.btr.Backend
{
	public class GameLoop
	{

		public GameLoop(IContext messageContext, RemoteDispatcherProxy remoteProxy)
		{
			this.MessageContext = messageContext;
			this.RemoteProxy = remoteProxy;
			this.IsRunning = false;
			this.IsShuttingDown = false;
		}

		public IContext MessageContext { get; private set; }
		public RemoteDispatcherProxy RemoteProxy { get; private set; }
		public bool IsRunning { get; private set; }
		public bool IsShuttingDown { get; private set; }

		/// <summary>
		/// Implements the main game loop. Will only return when the game loop have finished running and the process should shut down.
		/// </summary>
		public void Run()
		{
			IsRunning = true;

			while (!IsShuttingDown)
			{


				RemoteProxy.ForwardRemoteMessages();
				bool doAnotherDispatch = true;
				while (doAnotherDispatch)
				{
					doAnotherDispatch = MessageContext.Dispatcher.DispatchMessages();
					// TODO Add some tracking to the number of loops done, and log if it goes over some limits
				}
				// All of the local messages have been processed, send them off to the remote dispatcher as well
				RemoteProxy.SendToRemote();




			}

			IsRunning = false;
		}

		/// <summary>
		/// Starts the shutdown process of the main loop
		/// </summary>
		public void Shutdown()
		{
			IsShuttingDown = true;
		}
	}
}
