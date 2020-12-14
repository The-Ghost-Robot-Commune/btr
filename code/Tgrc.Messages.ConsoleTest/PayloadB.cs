using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages.ConsoleTest
{
	class PayloadB : IPayloadComponent
	{
		private static IPayloadComponentId id;

		public IPayloadComponentId Id { get { return id; } }

		public static void SetId(IPayloadComponentId id)
		{
			PayloadB.id = id;
		}
	}
}
