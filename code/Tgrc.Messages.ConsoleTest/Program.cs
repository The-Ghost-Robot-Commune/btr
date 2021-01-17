using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages.ConsoleTest
{
	static class Program
	{
		static void Main(string[] args)
		{

			SameProcessRemoteCommunicator();

		}

		private static void SameProcessRemoteCommunicator()
		{

		}


		private static void SimpleContextListener()
		{
			IContext context = CreateContext("TestContext");

			IDispatcher dispatcher = context.Dispatcher;
			IMessageComposer composer = context.MessageComposer;

			ListenerA listener = new ListenerA();

			dispatcher.RegisterListenerForAll(listener);


			PayloadA plA = new PayloadA();
			PayloadB plB = new PayloadB();

			var message = composer.Compose(plA, plB);
			dispatcher.Send(message);
			dispatcher.DispatchMessages();
		}

		private static IContext CreateContext(string contextName)
		{
			ContextFactory factory = new ContextFactory();
			var contextSetup = factory.Create(contextName, null);

			List<IPayloadComponentId> payloadIds = new List<IPayloadComponentId>();

			Assembly currentAssembly = Assembly.GetExecutingAssembly();
			var payloads = ContextUtilities.FindPayloadComponents(currentAssembly);
			foreach (var payload in payloads)
			{
				var id = contextSetup.RegisterPayloadComponent(payload);
				payloadIds.Add(id);

				if (payload.Name == nameof(PayloadA))
				{
					PayloadA.SetId(id);
				}
			}


			PayloadDefinition payloadBDefinition = new PayloadDefinition(nameof(PayloadB), typeof(PayloadB), ContextUtilities.FindSerializeMethod<PayloadB>(), ContextUtilities.FindDeserializeMethod<PayloadB>());
			var bId = contextSetup.RegisterPayloadComponent(payloadBDefinition);
			payloadIds.Add(bId);
			PayloadB.SetId(bId);
			return contextSetup.EndSetup();
		}
	}
}
