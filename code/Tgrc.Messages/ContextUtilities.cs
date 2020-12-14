using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	public static class ContextUtilities
	{
		public delegate bool PayloadFilter(Type payload, PayloadComponentAttribute payloadAttribute);

		private static readonly PayloadFilter alwaysIncludePayload = (t, a) => true;

		public static IEnumerable<Tuple<string, Type>> FindPayloadComponents(Assembly assembly)
		{
			return FindPayloadComponents(assembly, alwaysIncludePayload);
		}

		/// <summary>
		/// Find all payload components with the help of the PayloadComponentAttribute
		/// </summary>
		/// <param name="assembly"></param>
		/// <returns></returns>
		public static IEnumerable<Tuple<string, Type>> FindPayloadComponents(Assembly assembly, PayloadFilter includePayload)
		{
			var types = assembly.GetTypes();
			Type interfaceType = typeof(IPayloadComponent);
			foreach (var t in types)
			{
				if (interfaceType.IsAssignableFrom(t))
				{
					foreach (var componentAttribute in t.GetCustomAttributes<PayloadComponentAttribute>())
					{
						if (includePayload(t, componentAttribute))
						{
							yield return new Tuple<string, Type>(componentAttribute.Name, t);
						}
					}
				}
			}
		}
		
	}
}
