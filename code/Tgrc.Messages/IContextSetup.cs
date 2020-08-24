using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	public interface IContextSetup : IDisposable
	{
		void RegisterAssemblies(IEnumerable<Assembly> assemblies);

		IPayloadId RegisterPayload(string payloadName);

		IContext Finalize();
	}
}
