using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	public interface IContext
	{
		string Id { get; }

		
		IPayloadComponentId GetPayloadId(string payloadName);

		string GetPayloadName(IPayloadComponentId id);


		IMessageProxy CreateProxy(string name);

		IMessageProxy GetProxy(string name);

		IDispatcher CreateDispatcher(string name);

		IDispatcher GetDispatcher(string name);



	}
}
