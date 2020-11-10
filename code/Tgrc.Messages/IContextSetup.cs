using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{

	/// <summary>
	/// Any information feed to the setup instance must remain unmodified at least until EndSetup returns.
	/// </summary>
	public interface IContextSetup
	{
		
		void RegisterListener(Type listenerType, MethodInfo method);

		IPayloadComponentId RegisterPayloadComponent(string payloadComponentName, Type componentType);

		IContext EndSetup();
	}
}
