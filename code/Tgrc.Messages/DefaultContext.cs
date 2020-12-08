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
		private readonly Dictionary<string, PayloadDefinition> payloadDefinitions;
		private readonly Dictionary<IListener, HashSet<IPayloadComponentId>> listenerBookkeeping;

		public string Id { get; private set; }

		private DefaultContext(List<PayloadDefinition> payloads)
		{
			distributionLists = new DistributionList[payloads.Count];
			payloadDefinitions = new Dictionary<string, PayloadDefinition>(StringComparer.InvariantCulture);
			for (int i = 0; i < payloads.Count; i++)
			{
				var definition = payloads[i];

				if (definition.Id.Id != i)
				{
					throw new ContextSetupException(string.Format("Missmatch between payload id's. Definition '{0}' Index {1}", definition, i));
				}
				if (payloadDefinitions.ContainsKey(definition.Name))
				{
					throw new ContextSetupException(string.Format("Duplicate payload names '{0}'", definition.Name));
				}

				distributionLists[i] = new DistributionList(definition.Id);
				payloadDefinitions.Add(definition.Name, definition);
			}

			listenerBookkeeping = new Dictionary<IListener, HashSet<IPayloadComponentId>>();
		}

		
		public IPayloadComponentId FindPayloadId(string payloadName)
		{
			throw new NotImplementedException();
		}

		public string GetPayloadName(IPayloadComponentId id)
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
				var set = new HashSet<IPayloadComponentId>();
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

		public void DispatchMessages()
		{
			throw new NotImplementedException();
		}

		public void Send(IMessage message)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<IListener> GetAllListeners()
		{
			return listenerBookkeeping.Keys;
		}

		public IEnumerable<IPayloadComponentId> GetAllPayloadIds()
		{
			return payloadDefinitions.Values.Select(d => d.Id);
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

			public DistributionList(IPayloadComponentId payload)
			{
				this.Payload = payload;
				listeners = new HashSet<IListener>(InstanceEqualityComparer<IListener>.Instance);
			}

			public IPayloadComponentId Payload { get; private set; }

			public void AddListener(IListener listener)
			{
				listeners.Add(listener);
			}

			public void RemoveListener(IListener listener)
			{
				listeners.Remove(listener);
			}

			public void Invoke(IContext context, IMessage message)
			{
				// TODO catch exceptions for each listener
				foreach (var l in listeners)
				{
					l.HandleMessage(context, message);
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
