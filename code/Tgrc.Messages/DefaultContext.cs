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
		
		public string Id { get; private set; }

		private DefaultContext(IEnumerable<MethodInfo> methods, List<Tuple<string, Type>> payloads)
		{

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
			throw new NotImplementedException();
		}
		
		public void RegisterListener(IEnumerable<IListener> listeners, params IPayloadComponentId[] payloads)
		{
			throw new NotImplementedException();
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

			public int Id { get; set; }

			public bool Equals(IPayloadComponentId other)
			{
				return Id == ((PayloadId)other).Id;
			}

			public override bool Equals(object obj)
			{
				return obj is PayloadId && Equals((PayloadId)obj);
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


		private class Payload
		{

		}

		public class Setup : IContextSetup
		{
			private readonly List<MethodInfo> listenerMethods;
			private readonly List<Tuple<string, Type>> payloads;

			public Setup(string contextName, ILogger logger)
			{
				listenerMethods = new List<MethodInfo>();
				payloads = new List<Tuple<string, Type>>();
			}


			public void RegisterListener(MethodInfo method)
			{
				// TODO do some validation here for better exceptions

				listenerMethods.Add(method);
			}

			public IPayloadComponentId RegisterPayloadComponent(string payloadComponentName, Type componentType)
			{
				payloads.Add(new Tuple<string, Type>(payloadComponentName, componentType));

				return new PayloadId(payloads.Count - 1);
			}

			public IContext EndSetup()
			{
				return new DefaultContext(listenerMethods, payloads);
			}
		}
	}
}
