using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages.ConsoleTest
{
	[PayloadComponent(nameof(Tgrc.Messages.ConsoleTest.PayloadA))]
	class PayloadA : IPayloadComponent
	{
	}
}
