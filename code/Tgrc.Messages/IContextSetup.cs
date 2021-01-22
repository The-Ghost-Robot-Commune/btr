using System;
using System.Collections.Generic;
using System.IO;
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

		IPayloadComponentId RegisterPayloadComponent(IPayloadDefinition definition);

		IContext EndSetup();

		/// <summary>
		/// Enables a hash calculation based on all the payload definitions.
		/// Must be called before any definitions have been registered.
		/// </summary>
		void EnablePayloadDefinitionHash();
		byte[] GetPayloadDefinitionHash();
	}
}
