using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tgrc.Log;

namespace Tgrc.Messages
{
	class DefaultContext : IContext
	{

		private readonly DistributionList[] distributionLists;
		private readonly IReadOnlyDictionary<string, PayloadDefinition> payloadDefinitions;
		private readonly Dictionary<IListener, HashSet<IPayloadComponentId>> listenerBookkeeping;

		private int messageBufferIndex;
		private readonly List<IMessage>[] messageBuffer;

		private List<IMessage> CurrentBuffer { get { return messageBuffer[messageBufferIndex]; } }

		public string Id { get; private set; }

		private DefaultContext(List<PayloadDefinition> payloads)
		{
			distributionLists = new DistributionList[payloads.Count];
			var pd = new Dictionary<string, PayloadDefinition>(StringComparer.InvariantCulture);
			for (int i = 0; i < payloads.Count; i++)
			{
				var definition = payloads[i];

				if (definition.Id.Id != i)
				{
					throw new ContextSetupException(string.Format("Missmatch between payload id's. Definition '{0}' Index {1}", definition, i));
				}
				if (pd.ContainsKey(definition.Name))
				{
					throw new ContextSetupException(string.Format("Duplicate payload names '{0}'", definition.Name));
				}

				distributionLists[i] = new DistributionList(definition);
				pd.Add(definition.Name, definition);
			}
			payloadDefinitions = pd;

			listenerBookkeeping = new Dictionary<IListener, HashSet<IPayloadComponentId>>();

			messageBuffer = new List<IMessage>[2];
			messageBuffer[0] = new List<IMessage>();
			messageBuffer[1] = new List<IMessage>();
			messageBufferIndex = 0;
		}


		public IPayloadComponentId FindPayloadId(string payloadName)
		{
			PayloadDefinition result;
			if (payloadDefinitions.TryGetValue(payloadName, out result))
			{
				return result.Id;
			}
			return null;
		}
		
		public string GetPayloadName(IPayloadComponentId id)
		{
			throw new NotImplementedException();
		}

		public Type GetPayloadType(IPayloadComponentId id)
		{
			throw new NotImplementedException();
		}

		public void RegisterListener(IListener listener, params IPayloadComponentId[] payloads)
		{
			RegisterListener(listener, (IEnumerable<IPayloadComponentId>)payloads);
		}

		public void RegisterListener(IEnumerable<IListener> listeners, params IPayloadComponentId[] payloads)
		{
			RegisterListener(listeners, (IEnumerable<IPayloadComponentId>)payloads);
		}

		public void RegisterListener(IListener listener, IEnumerable<IPayloadComponentId> payloads)
		{
			var bookkeeping = FindOrCreateBookkeeping(listener);
			foreach (var p in payloads)
			{
				DistributionList list = FindOrCreateList(p);
				list.AddListener(listener);

				bookkeeping.Add(p);
			}
		}

		private DistributionList FindOrCreateList(IPayloadComponentId payload)
		{
			return distributionLists[payload.Id];
		}

		private HashSet<IPayloadComponentId> FindOrCreateBookkeeping(IListener listener)
		{
			if (listenerBookkeeping.ContainsKey(listener))
			{
				return listenerBookkeeping[listener];
			}
			else
			{
				var set = new HashSet<IPayloadComponentId>(PayloadComponentIdComparer.Instance);
				listenerBookkeeping.Add(listener, set);
				return set;
			}
		}

		public void RegisterListener(IEnumerable<IListener> listeners, IEnumerable<IPayloadComponentId> payloads)
		{
			foreach (var l in listeners)
			{
				RegisterListener(l, payloads);
			}
		}

		public void RegisterListenerForAll(IListener listener)
		{
			var bookkeeping = FindOrCreateBookkeeping(listener);
			foreach (var p in payloadDefinitions.Values)
			{
				var id = p.Id;
				DistributionList list = FindOrCreateList(id);
				list.AddListener(listener);

				bookkeeping.Add(id);
			}
		}

		public void RegisterListenerForAll(IEnumerable<IListener> listeners)
		{
			foreach (var l in listeners)
			{
				RegisterListenerForAll(l);
			}
		}

		public void RemoveListener(IListener listener)
		{
			var usedIds = listenerBookkeeping[listener];
			foreach (var id in usedIds)
			{
				DistributionList list = distributionLists[id.Id];
				list.RemoveListener(listener);
			}
			listenerBookkeeping.Remove(listener);
		}

		public void RemoveListeners(IEnumerable<IListener> listeners)
		{
			foreach (var l in listeners)
			{
				RemoveListener(l);
			}
		}

		public bool DispatchMessages()
		{
			// TODO Add more rigorous exception handling

			var messages = CurrentBuffer;
			messageBufferIndex = (messageBufferIndex == 0 ? 1 : 0);
			HashSet<IListener> usedListeners = new HashSet<IListener>(ReferenceEqualityComparer<IListener>.Instance);

			foreach (var m in messages)
			{
				usedListeners.Clear();

				foreach (var pid in m.GetPayloadComponentIds())
				{
					var list = distributionLists[pid.Id];

					list.Invoke(this, m, usedListeners);
				}
			}

			messages.Clear();
			// Check if we have gotten more messages as a result of the current dispatch batch
			return CurrentBuffer.Count > 0;
		}

		public void Send(IEnumerable<IMessage> messages)
		{
			CurrentBuffer.AddRange(messages);
		}

		public void Send(IMessage message)
		{
			CurrentBuffer.Add(message);
		}
		
		public IEnumerable<IPayloadComponentId> GetAllPayloadIds()
		{
			return payloadDefinitions.Values.Select(d => d.Id);
		}

		IEnumerable<Tuple<IPayloadComponentId, IEnumerable<IListener>>> IContext.GetAllListeners()
		{
			throw new NotImplementedException();
		}

		public IMessage Compose(params IPayloadComponent[] payloads)
		{
			return Compose((IEnumerable<IPayloadComponent>)payloads);
		}

		public IMessage Compose(IEnumerable<IPayloadComponent> payloads)
		{
			return new Message(payloads);
		}

		private class PayloadId : IPayloadComponentId
		{
			public PayloadId(int id)
			{
				this.Id = id;
			}

			public int Id { get; private set; }

			public bool Equals(IPayloadComponentId other)
			{
				return Id == other.Id;
			}

			public override bool Equals(object obj)
			{
				return obj is IPayloadComponentId && Equals((IPayloadComponentId)obj);
			}

			public override int GetHashCode()
			{
				return Id;
			}

			public override string ToString()
			{
				return Id.ToString();
			}
		}


		private class PayloadDefinition
		{
			public PayloadDefinition(string name, Type type, IPayloadComponentId id)
			{
				this.Name = name;
				this.Type = type;
				this.Id = id;
			}

			public string Name { get; private set; }
			public Type Type { get; private set; }
			public IPayloadComponentId Id { get; private set; }

			public override string ToString()
			{
				return string.Format("Name: {0} Type: {1} Id: {2}", Name, Type.FullName, Id.Id);
			}
		}

		private class DistributionList
		{
			private readonly HashSet<IListener> listeners;

			public DistributionList(PayloadDefinition payload)
			{
				this.Payload = payload;
				listeners = new HashSet<IListener>(ReferenceEqualityComparer<IListener>.Instance);
			}

			public PayloadDefinition Payload { get; private set; }

			public void AddListener(IListener listener)
			{
				listeners.Add(listener);
			}

			public void RemoveListener(IListener listener)
			{
				listeners.Remove(listener);
			}

			public void Invoke(IContext context, IMessage message, HashSet<IListener> usedListeners)
			{
				// TODO catch exceptions for each listener
				foreach (var l in listeners)
				{
					// Make sure each listener is only called once for each message
					if (usedListeners.Add(l))
					{
						l.HandleMessage(context, message); 
					}
				}
			}

		}

		public class Setup : IContextSetup
		{
			private readonly List<PayloadDefinition> payloads;

			public Setup(string contextName, ILogger logger)
			{
				payloads = new List<PayloadDefinition>();
			}

			public IPayloadComponentId RegisterPayloadComponent(string payloadComponentName, Type componentType)
			{
				IPayloadComponentId id = new PayloadId(payloads.Count);
				payloads.Add(new PayloadDefinition(payloadComponentName, componentType, id));

				return id;
			}

			public IContext EndSetup()
			{
				return new DefaultContext(payloads);
			}
		}
	}
}
