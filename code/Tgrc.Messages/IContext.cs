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

		
		IPayloadId GetPayloadId(string payloadName);

		string GetPayloadName(IPayloadId id);



	}
}
