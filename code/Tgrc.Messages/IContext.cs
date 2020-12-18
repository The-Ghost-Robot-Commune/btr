using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	/// <summary>
	/// 
	/// Payloads and listener types are registered too a specific context, and can only be used within it. They can only be registred during setup in order to allow for pre-processing.
	/// </summary>
	public interface IContext
	{
		string Id { get; }

		IDispatcher Dispatcher { get; }
		IRemoteDispatcher RemoteDispatcher { get; }
		IMessageComposer MessageComposer { get; }

		IPayloadComponentId FindPayloadId(string payloadName);

		string GetPayloadName(IPayloadComponentId id);
		Type GetPayloadType(IPayloadComponentId id);

		/// <summary>
		/// Intended for debugging. Can be slow
		/// </summary>
		/// <returns></returns>
		IEnumerable<IPayloadComponentId> GetAllPayloadIds();


	}
}
