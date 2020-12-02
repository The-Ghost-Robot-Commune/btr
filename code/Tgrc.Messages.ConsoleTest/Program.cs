using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages.ConsoleTest
{
	class Program
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
			}

			payloadIds.Add(contextSetup.RegisterPayloadComponent(nameof(PayloadB), typeof(PayloadB)));
			


			IContext context = contextSetup.EndSetup();



			ListenerA listener = new ListenerA();

			context.RegisterListener(listener, payloadIds[0]);

		}
	}
}
