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

			ContextFactory factory = new ContextFactory();
			var contextSetup = factory.Create("TestContext", null);

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


			IContext context = contextSetup.EndSetup();
			IDispatcher dispatcher = context.Dispatcher;
			IMessageComposer composer = context.MessageComposer;

			ListenerA listener = new ListenerA();

			dispatcher.RegisterListener(listener, payloadIds[0], bId);


			PayloadA plA = new PayloadA();
			PayloadB plB = new PayloadB();

			var message = composer.Compose(plA, plB);
			dispatcher.Send(message);
			dispatcher.DispatchMessages();


		}
	}
}
