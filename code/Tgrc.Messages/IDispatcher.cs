using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	public interface IDispatcher : IDIspatcherDebug
	{

		/// <summary>
		/// Registers a listener for messages containing the specified payloads.
		/// 
		/// A listener will get each message only once, even if the message is made up of multiple payloads that the listener is interested in.
		/// </summary>
		/// <param name="listener"></param>
		/// <param name="payloads"></param>
		void RegisterListener(IListener listener, params IPayloadComponentId[] payloads);
		void RegisterListener(IListener listener, IEnumerable<IPayloadComponentId> payloads);

		void RegisterListener(IEnumerable<IListener> listeners, params IPayloadComponentId[] payloads);
		void RegisterListener(IEnumerable<IListener> listeners, IEnumerable<IPayloadComponentId> payloads);

		/// <summary>
		/// Registers a listener for *all* payloads
		/// </summary>
		/// <param name="listener"></param>
		void RegisterListenerForAll(IListener listener);

		void RegisterListenerForAll(IEnumerable<IListener> listeners);

		void RemoveListener(IListener listener);
		void RemoveListeners(IEnumerable<IListener> listeners);


		void Send(IMessage message);
		void Send(IEnumerable<IMessage> messages);

		/// <summary>
		/// Send a message to all listeners as tho it is any other message. 
		/// Intended for messages originating from a remote dispatcher in another process
		/// </summary>
		/// <param name="message"></param>
		void ForwardSend(IMessage message);

		void ForwardSend(IEnumerable<IMessage> messages);

		/// <summary>
		/// Makes sure that the specified listener don't get sent any forwarded messages. This is to prevent message loops between server-client/frontend-backend.
		/// </summary>
		/// <param name="listener"></param>
		void AddToForwardIgnoreList(IListener listener);
		void AddToForwardIgnoreList(IEnumerable<IListener> listeners);
		

		/// <summary>
		/// Sends all the messages that have arrived since the last dispatch
		/// 
		/// If timeout requirements allow, doing the dispatch again is recommended if the return value is true.
		/// </summary>
		/// <returns>returns true if there have arrived more messages as a result of the current dispatch. </returns>
		bool DispatchMessages();



	}
}
