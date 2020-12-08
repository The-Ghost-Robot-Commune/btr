using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	public interface IMessage
	{

		int PayloadCount { get;}

		IEnumerable<IPayloadComponentId> GetPayloadComponentIds();

		/// <summary>
		/// Get's the payload of the specific "type", or null if none exist.
		/// </summary>
		/// <typeparam name="TPayload"></typeparam>
		/// <param name="id"></param>
		/// <returns></returns>
		TPayload GetPayload<TPayload>(IPayloadComponentId id)
			where TPayload : class, IPayloadComponent;

		TPayload GetPayload<TPayload>(int index)
			where TPayload : class, IPayloadComponent;

		bool HavePayload(IPayloadComponentId id);

	}
}
