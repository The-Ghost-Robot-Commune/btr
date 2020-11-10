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

		private static readonly Func<Type, PayloadComponentAttribute, bool> alwaysInclude = (t, a) => true;

		public static IEnumerable<Tuple<string, Type>> FindPayloadComponents(Assembly assembly)
		{
			return FindPayloadComponents(assembly, alwaysInclude);
		}

		/// <summary>
		/// Find all payload components with the help of the PayloadComponentAttribute
		/// </summary>
		/// <param name="assembly"></param>
		/// <returns></returns>
		public static IEnumerable<Tuple<string, Type>> FindPayloadComponents(Assembly assembly, Func<Type, PayloadComponentAttribute, bool> includePayload)
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

	}
}
