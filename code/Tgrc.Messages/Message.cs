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

		public Message(IEnumerable<Tuple<IPayloadComponentId, IPayloadComponent>> payloads)
		{
			this.payloads = new Dictionary<IPayloadComponentId, IPayloadComponent>();
			foreach (var p in payloads)
			{
				this.payloads.Add(p.Item1, p.Item2);
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
