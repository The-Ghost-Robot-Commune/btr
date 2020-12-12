using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages.ConsoleTest
{
	class ListenerA : IListener
	{
		public void HandleMessage(IContext sender, IMessage message)
		{
			throw new NotImplementedException();
		}
	}
}
