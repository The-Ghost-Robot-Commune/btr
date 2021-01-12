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
		
		IPayloadComponentId RegisterPayloadComponent(string payloadComponentName, Type componentType, Func<IPayloadComponent, byte[]> serializer, Func<byte[], IPayloadComponent> deserializer);

		IContext EndSetup();
	}
}
