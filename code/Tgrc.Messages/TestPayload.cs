using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	[PayloadComponent(nameof(TestPayload))]
	[MessagePackObject]
	public class TestPayload : IPayloadComponent
	{
		private static IPayloadComponentId staticId;

		[IgnoreMember]
		public IPayloadComponentId Id { get { return staticId; } }

		[Key(0)]
		public string DummyValue { get; set; }

		public static void SetId(IPayloadComponentId id)
		{
			staticId = id;
		}

		public override string ToString()
		{
			return DummyValue;
		}
	}
}
