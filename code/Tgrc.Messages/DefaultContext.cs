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
		private Dictionary<string, PayloadDefinition> payloadDefinitions;

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
		}

		public IEnumerable<Type> GetListenerTypes()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<IPayloadComponentId> GetPayloadIds()
		{
			throw new NotImplementedException();
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
			foreach (var p in payloads)
			{
				DistributionList list = FindOrCreateList(p);
				list.AddListener(listener);
			}
		}

		private DistributionList FindOrCreateList(IPayloadComponentId payload)
		{
			return distributionLists[payload.Id];
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
			throw new NotImplementedException();
		}

		public void RegisterListenerForAll(IEnumerable<IListener> listeners)
		{
			throw new NotImplementedException();
		}

		public void DispatchMessages()
		{
			throw new NotImplementedException();
		}

		public void Send(IMessage message)
		{
			throw new NotImplementedException();
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
			private HashSet<IListener> listeners;

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
