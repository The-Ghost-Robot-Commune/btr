using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroFormatter;

namespace Tgrc.Messages.ConsoleTest
{
	[ZeroFormattable]
	public class PayloadB : IPayloadComponent
	{
		private static IPayloadComponentId id;

		[IgnoreFormat]
		public IPayloadComponentId Id { get { return id; } }

		public static void SetId(IPayloadComponentId id)
		{
			PayloadB.id = id;
		}
	}
}
