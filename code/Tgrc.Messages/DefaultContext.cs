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

		public IDispatcher CreateDispatcher(string name)
		{
			throw new NotImplementedException();
		}

		public IMessageProxy CreateProxy(string name)
		{
			throw new NotImplementedException();
		}

		public IDispatcher GetDispatcher(string name)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Type> GetListenerTypes()
		{
			throw new NotImplementedException();
		}

		public IPayloadComponentId GetPayloadId(string payloadName)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<IPayloadComponentId> GetPayloadIds()
		{
			throw new NotImplementedException();
		}

		public string GetPayloadName(IPayloadComponentId id)
		{
			throw new NotImplementedException();
		}

		public IMessageProxy GetProxy(string name)
		{
			throw new NotImplementedException();
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
				throw new NotImplementedException();
			}

			public IContext EndSetup()
			{
				throw new NotImplementedException();
			}
		}
	}
}
