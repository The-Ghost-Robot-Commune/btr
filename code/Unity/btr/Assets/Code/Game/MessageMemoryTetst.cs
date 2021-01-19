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

		public string PayloadMessage;

		private RemoteDispatcherProxy proxy;
		IContext context;
		DateTime lastSend;

		void Start()
		{

			string contextName = "Context" + (IsHost ? ".Host" : ".Client");
			context = CreateContext(contextName);


			DotNetThreadStarter threadStarter = new DotNetThreadStarter();
			MemoryMappedCommunicator communicator = new MemoryMappedCommunicator(threadStarter, "Tgrc.Messages.MemoryMap", IsHost);

			proxy = CreateRemoteDispatcher(context, communicator);

			lastSend = DateTime.UtcNow;

			communicator.StartThreads();
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

		private IMessage CreateBasicMessage()
		{
			TestPayload payload = new TestPayload();
			payload.DummyValue = PayloadMessage;

			return context.MessageComposer.Compose(payload);
		}

		void Update()
		{
			if ((DateTime.UtcNow - lastSend) > TimeSpan.FromSeconds(2.0))
			{
				IMessage message = CreateBasicMessage();
				context.Dispatcher.Send(message);

				lastSend = DateTime.UtcNow;
			}

			context.Dispatcher.DispatchMessages();
			proxy.ForwardRemoteMessages();

			Debug.Log("Update");

		}
	}
}
