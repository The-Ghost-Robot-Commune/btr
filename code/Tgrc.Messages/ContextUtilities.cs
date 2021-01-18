using MessagePack;
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
		private static readonly Dictionary<Type, Serialize> serializeCache = new Dictionary<Type, Serialize>();
		private static readonly Dictionary<Type, Deserialize> deserializeCache = new Dictionary<Type, Deserialize>();

		private static readonly Type[] emptyTypeList = new Type[0];

		public static IEnumerable<PayloadDefinition> FindPayloadComponents(Assembly assembly)
		{
			return FindPayloadComponents(assembly, alwaysIncludePayload);
		}

		/// <summary>
		/// Find all payload components with the help of the PayloadComponentAttribute
		/// </summary>
		/// <param name="assembly"></param>
		/// <returns></returns>
		public static IEnumerable<PayloadDefinition> FindPayloadComponents(Assembly assembly, PayloadFilter includePayload)
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
							Serialize serialize = FindSerializeMethod(t);
							Deserialize deserialize = FindDeserializeMethod(t);
							yield return new PayloadDefinition(componentAttribute.Name, t, serialize, deserialize);
						}
					}
				}
			}
		}


		public static Serialize FindSerializeMethod(Type payloadType)
		{
			Serialize value;
			if (!serializeCache.TryGetValue(payloadType, out value))
			{
				MethodInfo findSerializeGeneric = typeof(ContextUtilities).GetMethod(nameof(FindSerializeMethod), emptyTypeList);
				MethodInfo findSerializeFinal = findSerializeGeneric.MakeGenericMethod(payloadType);
				value = (Serialize)findSerializeFinal.Invoke(null, null);
			}
			return value;
		}

		public static Serialize FindSerializeMethod<TPayload>()
			where TPayload : class, IPayloadComponent
		{
			Serialize value;
			if (!serializeCache.TryGetValue(typeof(TPayload), out value))
			{
				value = (p, s) => MessagePackSerializer.Serialize(s, (TPayload)p);
				serializeCache.Add(typeof(TPayload), value);
			}
			return value;
		}

		public static Deserialize FindDeserializeMethod(Type payloadType)
		{
			Deserialize value;
			if (!deserializeCache.TryGetValue(payloadType, out value))
			{
				MethodInfo findDeserializeGeneric = typeof(ContextUtilities).GetMethod(nameof(FindDeserializeMethod), emptyTypeList);
				MethodInfo findDeserializeFinal = findDeserializeGeneric.MakeGenericMethod(payloadType);
				value = (Deserialize)findDeserializeFinal.Invoke(null, null);
			}
			return value;
		}

		public static Deserialize FindDeserializeMethod<TPayload>()
			where TPayload : class, IPayloadComponent
		{
			Deserialize value;
			if (!deserializeCache.TryGetValue(typeof(TPayload), out value))
			{
				value = (s) => MessagePackSerializer.Deserialize<TPayload>(s);
				deserializeCache.Add(typeof(TPayload), value);
			}
			return value;
		}

	}
}
