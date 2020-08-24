using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	public interface IDispatcher
	{

		void Send(IMessage message);

		/// <summary>
		/// Sends all the messages that have arrived since the last dispatch
		/// </summary>
		void DispatchMessages();
	}
}
