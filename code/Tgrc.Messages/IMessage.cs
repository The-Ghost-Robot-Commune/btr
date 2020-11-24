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

		IEnumerable<IPayloadComponentId> GetPayloadComponents();

		TPayload GetPayload<TPayload>(IPayloadComponentId id)
			where TPayload : class, IPayloadComponent;

		TPayload GetPayload<TPayload>(int index)
			where TPayload : class, IPayloadComponent;

		bool HavePayload(IPayloadComponentId id);

	}
}
