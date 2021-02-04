using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tgrc.Messages;
using Tgrc.Thread;

namespace Tgrc.btr.Backend
{
	public class GameLoopSetup
	{
		private IThreadStarter threadStarter;
		private string contextName;

		public GameLoopSetup()
		{ }

		public void SetMainConfiguration()
		{

		}

		public void AddThreadStarter(IThreadStarter threadStarter)
		{
			this.threadStarter = threadStarter;
		}



		public GameLoop CreateGameLoop()
		{
			if (threadStarter == null)
			{
				throw new InvalidOperationException(string.Format("Missing {0}", typeof(IThreadStarter).FullName));
			}
			// Validate that we have gotten all the external settings needed. Like configuration file, communicator template/factory, ...

			IContext messageContext = CreateMessageContext();
			IRemoteCommunicator communicator = CreateContextCommunicator();
			RemoteDispatcherProxy remoteProxy = CreateRemoteDispatcher(messageContext, communicator);

			return new GameLoop(messageContext, remoteProxy);
		}

		private IRemoteCommunicator CreateContextCommunicator()
		{
			MemoryMappedCommunicator communicator = new MemoryMappedCommunicator(threadStarter, "Tgrc.btr.MemoryMap", true);
			return communicator;
		}

		private IContext CreateMessageContext()
		{
			ContextFactory factory = new ContextFactory();
			var contextSetup = factory.Create(contextName, null);

			contextSetup.EnablePayloadDefinitionHash();

			List<IPayloadComponentId> payloadIds = new List<IPayloadComponentId>();

			Assembly currentAssembly = Assembly.GetAssembly(typeof(TestPayload));
			var payloads = ContextUtilities.FindPayloadComponents(currentAssembly);
			foreach (var payload in payloads)
			{
				var id = contextSetup.RegisterPayloadComponent(payload);
				payloadIds.Add(id);
			}

			IContext context = contextSetup.EndSetup();

			// TODO Validate that this context hash matches the other frontend context.

			return context;
		}

		private RemoteDispatcherProxy CreateRemoteDispatcher(IContext context, IRemoteCommunicator communicator)
		{
			RemoteDispatcherProxy proxy = new RemoteDispatcherProxy(context.Dispatcher, context.Serializer, communicator);

			return proxy;
		}
	}
}
