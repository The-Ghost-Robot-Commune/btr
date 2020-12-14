using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	public interface IMessageComposer
	{
		IMessage Compose(params IPayloadComponent[] payloads);

		IMessage Compose(IEnumerable<IPayloadComponent> payloads);
	}
}
