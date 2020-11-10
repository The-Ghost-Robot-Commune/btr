using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	public interface IContextSetup
	{
		
		void RegisterListener(Type listenerType);

		IPayloadComponentId RegisterPayloadComponent(string payloadComponentName, Type componentType);

		IContext EndSetup();
	}
}
