using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	public interface IDispatcher
	{

		/// <summary>
		/// Registers a listener for messages containing the specified payloads.
		/// </summary>
		/// <param name="listener"></param>
		/// <param name="payloads"></param>
		void RegisterListener(IListener listener, params IPayloadComponentId[] payloads);

		void RegisterListener(IEnumerable<IListener> listeners, params IPayloadComponentId[] payloads);

		/// <summary>
		/// Registers a listener for *all* payloads
		/// </summary>
		/// <param name="listener"></param>
		void RegisterListenerForAll(IListener listener);

		void RegisterListenerForAll(IEnumerable<IListener> listeners);


		void Send(IMessage message);

		/// <summary>
		/// Sends all the messages that have arrived since the last dispatch
		/// </summary>
		void DispatchMessages();
	}
}
