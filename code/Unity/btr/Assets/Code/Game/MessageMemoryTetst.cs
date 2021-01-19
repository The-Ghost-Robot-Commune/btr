using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tgrc.Messages;
using Tgrc.Thread;
using UnityEngine;

namespace Tgrc.btr
{
	class MessageMemoryTetst : MonoBehaviour
	{
		public bool IsHost;

		private RemoteDispatcherProxy proxy;
		IContext context;

		void Start()
		{

			string contextName = "Context" + (IsHost ? ".Host" : ".Client");
			context = CreateContext(contextName);


			DotNetThreadStarter threadStarter = new DotNetThreadStarter();
			MemoryMappedCommunicator communicator = new MemoryMappedCommunicator(threadStarter, "Tgrc.Messages.MemoryMap", IsHost);

			proxy = CreateRemoteDispatcher(context, communicator);

			communicator.StartThreads();

			if (IsHost)
			{
				IMessage message = CreateBasicMessage(context);
				context.Dispatcher.Send(message);
			}


		}

		private static IContext CreateContext(string contextName)
		{
			ContextFactory factory = new ContextFactory();
			var contextSetup = factory.Create(contextName, null);

			List<IPayloadComponentId> payloadIds = new List<IPayloadComponentId>();

			Assembly currentAssembly = Assembly.GetAssembly(typeof(TestPayload));
			var payloads = ContextUtilities.FindPayloadComponents(currentAssembly);
			foreach (var payload in payloads)
			{
				var id = contextSetup.RegisterPayloadComponent(payload);
				payloadIds.Add(id);
			}

			IContext c = contextSetup.EndSetup();
			TestPayload.SetId(c.FindPayloadId(nameof(TestPayload)));

			return c;
		}

		private static RemoteDispatcherProxy CreateRemoteDispatcher(IContext context, IRemoteCommunicator communicator)
		{
			RemoteDispatcherProxy p = new RemoteDispatcherProxy(context.Dispatcher, context.Serializer, communicator);
			UnityWriterListener listener = new UnityWriterListener();
			context.Dispatcher.RegisterListenerForAll(listener);
			return p;
		}

		private static IMessage CreateBasicMessage(IContext context)
		{
			TestPayload payload = new TestPayload();
			payload.DummyValue = "ConsoleTest";

			return context.MessageComposer.Compose(payload);
		}

		void Update()
		{

			context.Dispatcher.DispatchMessages();
			proxy.ForwardRemoteMessages();

		}
	}
}
