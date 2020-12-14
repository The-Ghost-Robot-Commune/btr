using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages.ConsoleTest
{
	[PayloadComponent(nameof(Tgrc.Messages.ConsoleTest.PayloadA))]
	[PayloadComponent(nameof(Tgrc.Messages.ConsoleTest.PayloadA) + ".Alt")]
	class PayloadA : IPayloadComponent
	{
		private static IPayloadComponentId id;

		public IPayloadComponentId Id { get { return id; } }

		public static void SetId(IPayloadComponentId id)
		{
			PayloadA.id = id;
		}
	}
}
