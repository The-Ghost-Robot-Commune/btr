using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages.ConsoleTest
{
	[PayloadComponent(nameof(Tgrc.Messages.ConsoleTest.PayloadA))]
	[PayloadComponent(nameof(Tgrc.Messages.ConsoleTest.PayloadA) + ".Alt")]
	[MessagePackObject]
	public class PayloadA : IPayloadComponent
	{
		private static IPayloadComponentId id;

		[IgnoreMember]
		public IPayloadComponentId Id { get { return id; } }

		[Key(0)]
		public string DummyValue { get; set; }

		public static void SetId(IPayloadComponentId id)
		{
			PayloadA.id = id;
		}
	}
}
