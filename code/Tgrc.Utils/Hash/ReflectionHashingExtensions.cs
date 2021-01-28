using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Hash
{
	public static class ReflectionHashingExtensions
	{
		private static readonly byte[] ExplicitNull = BitConverter.GetBytes(byte.MinValue);
		private static readonly byte[] ExplicitTrue = BitConverter.GetBytes(true);
		private static readonly byte[] ExplicitFalse = BitConverter.GetBytes(false);

		private enum AttributeBehavior
		{
			Include,
			Exclude
		}

		public static byte[] CalculateClassHash(this HashAlgorithm algorithm, Type value)
		{
			algorithm.Initialize();
			algorithm.AppendClass(value, true);
			return algorithm.Hash;
		}

		public static byte[] CalculateTypeHash(this HashAlgorithm algorithm, Type value)
		{
			algorithm.Initialize();
			algorithm.AppendType(value, true);
			return algorithm.Hash;
		}

		/// <summary>
		/// If two class types have the same name, interface, and attributes (not including the values in the attribute), they generate the same hash.
		/// This does *NOT* mean that they are implemented the same, just that they look the same on the surface.
		/// </summary>
		/// <param name="algorithm"></param>
		/// <param name="value"></param>
		/// <param name="isFinalAppend"></param>
		public static void AppendClass(this HashAlgorithm algorithm, Type value, bool isFinalAppend = false)
		{
			var methods = value.GetMethods();
			foreach (var m in methods)
			{
				algorithm.Append(m);
			}
			algorithm.Append(methods.Length);

			var properties = value.GetProperties();
			foreach (var p in properties)
			{
				algorithm.Append(p);
			}
			algorithm.Append(properties.Length);

			algorithm.Append(value, AttributeBehavior.Include, isFinalAppend);
		}

		/// <summary>
		/// Calculates a hash of the specified type. 
		/// </summary>
		/// <param name="algorithm"></param>
		/// <param name="value"></param>
		/// <param name="isFinalAppend"></param>
		public static void AppendType(this HashAlgorithm algorithm, Type value, bool isFinalAppend = false)
		{
			algorithm.Append(value, AttributeBehavior.Include, isFinalAppend);
		}

		private static void Append(this HashAlgorithm algorithm, Type value, AttributeBehavior attributeBehavior, bool isFinalAppend = false)
		{
			if (value.IsGenericType)
				algorithm.Append(ExplicitTrue);
			else
				algorithm.Append(ExplicitFalse);
			if (value.IsGenericTypeDefinition)
				algorithm.Append(ExplicitTrue);
			else
				algorithm.Append(ExplicitFalse);

			// TODO Add more explicit type info for generic versions
			if (value.IsGenericParameter)
				algorithm.Append(ExplicitTrue);
			else
				algorithm.Append(ExplicitFalse);

			if (value.ContainsGenericParameters)
				algorithm.Append(ExplicitTrue);
			else
				algorithm.Append(ExplicitFalse);


			if (value.IsArray)
			{
				algorithm.Append(ExplicitTrue);
				algorithm.AppendType(value.GetElementType());
			}
			else
			{
				algorithm.Append(ExplicitFalse);
			}

			algorithm.AppendMemberInfo(value, attributeBehavior, isFinalAppend);
		}

		public static void Append(this HashAlgorithm algorithm, AssemblyName value, bool isFinalAppend = false)
		{
			algorithm.Append(value.Name);
			algorithm.Append(value.FullName);
			algorithm.Append(value.Version.ToString(), isFinalAppend);
		}

		private static void Append(this HashAlgorithm algorithm, Attribute value, bool isFinalAppend = false)
		{
			Type attributeType = value.GetType();

			algorithm.Append(attributeType, AttributeBehavior.Exclude, isFinalAppend);
		}

		private static void Append(this HashAlgorithm algorithm, PropertyInfo value, bool isFinalAppend = false)
		{
			algorithm.AppendMemberInfo(value, AttributeBehavior.Include);
			var indexParameters = value.GetIndexParameters();
			foreach (var i in indexParameters)
			{
				algorithm.Append(i);
			}
			algorithm.Append(indexParameters.Length);

			if (value.GetMethod == null)
			{
				algorithm.Append(ExplicitNull);
			}
			else
			{
				algorithm.Append(value.GetMethod);
			}
			if (value.SetMethod == null)
			{
				algorithm.Append(ExplicitNull);
			}
			else
			{
				algorithm.Append(value.SetMethod);
			}

			algorithm.AppendType(value.PropertyType, isFinalAppend);
		}

		private static void Append(this HashAlgorithm algorithm, MethodInfo value, bool isFinalAppend = false)
		{
			algorithm.AppendMemberInfo(value, AttributeBehavior.Include);
			algorithm.Append((int)value.Attributes);

			if (value.IsGenericMethod)
			{
				algorithm.Append(ExplicitTrue);
				if (value.IsGenericMethodDefinition)
				{
					algorithm.Append(ExplicitTrue);
				}
				else
				{
					algorithm.Append(ExplicitFalse);
				}

				var genericArguments = value.GetGenericArguments();
				foreach (var arg in genericArguments)
				{
					algorithm.AppendType(arg);
				}
				algorithm.Append(genericArguments.Length);
			}
			else
			{
				algorithm.Append(ExplicitFalse);
			}

			var parameters = value.GetParameters();
			foreach (var p in parameters)
			{
				algorithm.Append(p);
			}
			algorithm.Append(parameters.Length);

			algorithm.AppendType(value.ReturnType, isFinalAppend);
		}

		private static void Append(this HashAlgorithm algorithm, ParameterInfo value, bool isFinalAppend = false)
		{
			algorithm.Append((int)value.Attributes);
			algorithm.Append(value.Name);
			algorithm.Append(value.Position);
			int attributeCount = 0;
			foreach (var a in value.GetCustomAttributes<Attribute>())
			{
				algorithm.Append(a);
				++attributeCount;
			}
			algorithm.Append(attributeCount);
			algorithm.AppendType(value.ParameterType, isFinalAppend);
		}

		
		/// <summary>
		/// Calculates a hash of a Delegate object. The resulting hash can be used to validate that two different Delegate instanses
		/// are setup in the same way (meaning they have the same type of targets and in the same order).
		/// The hash value is also stable outside of the current process lifetime, so it can be used to validate saved data.
		/// </summary>
		/// <param name="algorithm"></param>
		/// <param name="value"></param>
		/// <param name="isFinalAppend"></param>
		public static void Append(this HashAlgorithm algorithm, Delegate value, bool isFinalAppend = false)
		{
			var invocationList = value.GetInvocationList();
			foreach (var d in invocationList)
			{
				if (d.Target == null)
				{
					algorithm.Append(ExplicitNull);
				}
				else
				{
					algorithm.AppendType(d.Target.GetType());
				}
				algorithm.Append(d.Method);
			}
			algorithm.Append(invocationList.Length, isFinalAppend);
		}

		private static void AppendMemberInfo(this HashAlgorithm algorithm, MemberInfo value, AttributeBehavior attributeBehavior, bool isFinalAppend = false)
		{
			algorithm.Append((int)value.MemberType);

			int attributeCount = 0;
			if (attributeBehavior == AttributeBehavior.Include)
			{
				foreach (var a in value.GetCustomAttributes<Attribute>())
				{
					algorithm.Append(a);
					++attributeCount;
				}
			}
			// Always add the attribute count. This way a type without any attributes hashes the same no matter what the parameter says
			algorithm.Append(attributeCount);

			algorithm.Append(value.Name, isFinalAppend);
		}
	}
}
