using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	class Message : IMessage
	{
		private readonly Dictionary<IPayloadComponentId, IPayloadComponent> payloads;

		public Message(IEnumerable<IPayloadComponent> payloads)
		{
			this.payloads = new Dictionary<IPayloadComponentId, IPayloadComponent>(PayloadComponentIdComparer.Instance);
			foreach (var p in payloads)
			{
				this.payloads.Add(p.Id, p);
			}
		}

		public int PayloadCount { get { return payloads.Count; } }

		public IEnumerable<IPayloadComponentId> GetPayloadComponentIds()
		{
			return payloads.Keys;
		}

		public IEnumerable<IPayloadComponent> GetPayloadComponents()
		{
			return payloads.Values;
		}

		public bool HavePayload(IPayloadComponentId id)
		{
			return payloads.ContainsKey(id);
		}

		TPayload IMessage.GetPayload<TPayload>(IPayloadComponentId id)
		{
			if (!payloads.ContainsKey(id))
			{
				return null;
			}

			return (TPayload)payloads[id];
		}
	}
}
