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

		public static byte[] CalculateHash(this HashAlgorithm algorithm, Type value)
		{
			algorithm.Initialize();
			algorithm.Append(value, true);
			return algorithm.Hash;
		}

		public static void Append(this HashAlgorithm algorithm, Type value, bool isFinalAppend = false)
		{
			algorithm.Append((int)value.Attributes);
			algorithm.Append(value.Assembly.GetName());
			var fullName = value.FullName;
			if (fullName == null)
			{
				algorithm.Append(ExplicitNull);
			}
			else
			{
				algorithm.Append(fullName);
			}

			if (value.IsGenericType)
				algorithm.Append(ExplicitTrue);
			else
				algorithm.Append(ExplicitFalse);
			if (value.IsGenericParameter)
				algorithm.Append(ExplicitTrue);
			else
				algorithm.Append(ExplicitFalse);
			if (value.IsGenericTypeDefinition)
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
				algorithm.Append(value.GetElementType());
			}
			else
			{
				algorithm.Append(ExplicitFalse);
			}

			algorithm.Append((MemberInfo)value, true, isFinalAppend);
		}

		public static void Append(this HashAlgorithm algorithm, AssemblyName value, bool isFinalAppend = false)
		{
			algorithm.Append(value.FullName);
			algorithm.Append(value.Version.ToString(), isFinalAppend);
		}

		public static void Append(this HashAlgorithm algorithm, Attribute value, bool isFinalAppend = false)
		{
			Type attributeType = value.GetType();

			algorithm.Append(attributeType.FullName);
			algorithm.Append((MemberInfo)attributeType, false, isFinalAppend);

		}

		public static void Append(this HashAlgorithm algorithm, PropertyInfo value, bool isFinalAppend = false)
		{
			algorithm.Append((MemberInfo)value, true);
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

			algorithm.Append(value.PropertyType, isFinalAppend);
		}

		public static void Append(this HashAlgorithm algorithm, MethodInfo value, bool isFinalAppend = false)
		{
			algorithm.Append((MemberInfo)value, true);
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
					algorithm.Append(arg);
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

			algorithm.Append(value.ReturnType, isFinalAppend);
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
			algorithm.Append(value.ParameterType, isFinalAppend);
		}

		public static void Append(this HashAlgorithm algorithm, MemberInfo value, bool includeAttributes, bool isFinalAppend = false)
		{
			algorithm.Append((int)value.MemberType);
			if (value.DeclaringType == null)
			{
				algorithm.Append(ExplicitNull);
			}
			else
			{
				algorithm.Append(value.DeclaringType);
			}
			if (includeAttributes)
			{
				int attributeCount = 0;
				foreach (var a in value.GetCustomAttributes<Attribute>())
				{
					algorithm.Append(a);
					++attributeCount;
				}
				algorithm.Append(attributeCount);
			}
			algorithm.Append(value.Name, isFinalAppend);
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
					algorithm.Append(d.Target.GetType());
				}
				algorithm.Append(d.Method);
			}
			algorithm.Append(invocationList.Length, isFinalAppend);
		}
	}
}
