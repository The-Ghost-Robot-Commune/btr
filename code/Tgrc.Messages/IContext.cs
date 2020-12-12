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
	public interface IContext : IDispatcher, IMessageComposer
	{
		string Id { get; }

		
		IPayloadComponentId FindPayloadId(string payloadName);
		IPayloadComponentId FindPayloadId(Type payloadType);

		string GetPayloadName(IPayloadComponentId id);
		Type GetPayloadType(IPayloadComponentId id);

		/// <summary>
		/// Intended for debugging. Can be slow
		/// </summary>
		/// <returns></returns>
		IEnumerable<IPayloadComponentId> GetAllPayloadIds();

		

		/// <summary>
		/// *DEBUG*
		/// Intended for debugging, might be very slow.
		/// </summary>
		/// <returns></returns>
		IEnumerable<Tuple<IPayloadComponentId, IEnumerable<IListener>>> GetAllListeners();
	}
}
