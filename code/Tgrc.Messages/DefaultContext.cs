using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

		public IPayloadComponentId GetPayloadId(string payloadName)
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
			public Setup(string contextName)
			{

			}

			public IContext EndSetup()
			{
				throw new NotImplementedException();
			}

			public void RegisterListener(Type listenerType)
			{
				throw new NotImplementedException();
			}
			
			public IPayloadComponentId RegisterPayloadComponent(string payloadComponentName, Type componentType)
			{
				throw new NotImplementedException();
			}
		}
	}
}
