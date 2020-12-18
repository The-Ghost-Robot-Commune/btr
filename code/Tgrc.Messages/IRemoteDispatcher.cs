using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	public interface IRemoteDispatcher
	{
		/// <summary>
		/// Send a message to all listeners as tho it is any other message. 
		/// Intended for messages originating from a remote dispatcher in another process
		/// </summary>
		/// <param name="message"></param>
		void RemoteSend(IMessage message);

		void RemoteSend(IEnumerable<IMessage> messages);
	}
}
