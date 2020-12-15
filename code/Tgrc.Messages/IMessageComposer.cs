using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	public interface IMessageComposer
	{
		// TODO Add tags to messages so they can easily be directed to only a subset of listeners. Useful for request/response patterns

		IMessage Compose(params IPayloadComponent[] payloads);

		IMessage Compose(IEnumerable<IPayloadComponent> payloads);
	}
}
