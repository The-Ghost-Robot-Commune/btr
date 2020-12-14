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
				var id = contextSetup.RegisterPayloadComponent(payload.Item1, payload.Item2);
				payloadIds.Add(id);

				if (payload.Item1 == nameof(PayloadA))
				{
					PayloadA.SetId(id);
				}
			}


			var bId = contextSetup.RegisterPayloadComponent(nameof(PayloadB), typeof(PayloadB));
			payloadIds.Add(bId);
			PayloadB.SetId(bId);


			IContext context = contextSetup.EndSetup();

			ListenerA listener = new ListenerA();

			context.RegisterListener(listener, payloadIds[0], bId);


			PayloadA plA = new PayloadA();
			PayloadB plB = new PayloadB();

			var message = context.Compose(plA, plB);
			context.Send(message);
			context.DispatchMessages();


		}
	}
}
