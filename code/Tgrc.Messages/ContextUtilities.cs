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
		public delegate bool ListenerMethodFilter(Type listener, MethodInfo method, ListenerMethodAttribute methodAttribute);
		public delegate bool PayloadFilter(Type payload, PayloadComponentAttribute payloadAttribute);

		private static readonly ListenerMethodFilter alwaysIncludeMethod = (l, m, a) => true;
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
				if (t.IsAssignableFrom(interfaceType))
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


		public static IEnumerable<MethodInfo> FindListenerMethods(Assembly assembly)
		{
			return FindListenerMethods(assembly, alwaysIncludeMethod);
		}

		public static IEnumerable<MethodInfo> FindListenerMethods(Assembly assembly, ListenerMethodFilter includeMethod)
		{
			Type interfaceType = typeof(IListener);
			foreach (var t in assembly.GetTypes())
			{
				if (t.IsClass && t.IsAssignableFrom(interfaceType))
				{
					// The type is a IListener implementor, now check the methods
					foreach (var method in t.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
					{
						// Check for the existance of the attribute and the optional filter
						var customAttribute = method.GetCustomAttribute<ListenerMethodAttribute>();
						if (customAttribute != null && includeMethod(t, method, customAttribute))
						{
							yield return method;
						}
					}
				}
			}
		}

	}
}
