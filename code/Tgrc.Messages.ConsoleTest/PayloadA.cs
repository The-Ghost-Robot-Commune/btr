using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroFormatter;

namespace Tgrc.Messages.ConsoleTest
{
	[PayloadComponent(nameof(Tgrc.Messages.ConsoleTest.PayloadA))]
	[PayloadComponent(nameof(Tgrc.Messages.ConsoleTest.PayloadA) + ".Alt")]
	[ZeroFormattable]
	public class PayloadA : IPayloadComponent
	{
		private static IPayloadComponentId id;

		[IgnoreFormat]
		public IPayloadComponentId Id { get { return id; } }

		[Index(0)]
		public virtual string DummyValue { get; set; }

		public static void SetId(IPayloadComponentId id)
		{
			PayloadA.id = id;
		}
	}
}
