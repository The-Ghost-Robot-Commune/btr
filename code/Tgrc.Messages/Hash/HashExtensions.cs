using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Tgrc.Messages.Hash
{
	public static class HashExtensions
	{
		private static readonly byte[] ExplicitNull = BitConverter.GetBytes(byte.MinValue);
		private static readonly byte[] ExplicitTrue = BitConverter.GetBytes(true);
		private static readonly byte[] ExplicitFalse = BitConverter.GetBytes(false);


		public static byte[] CalculateHash(this HashAlgorithm algorithm, string value)
		{
			algorithm.Initialize();
			algorithm.Append(value, true);
			return algorithm.Hash;
		}

		public static byte[] CalculateHash(this HashAlgorithm algorithm, bool value)
		{
			algorithm.Initialize();
			algorithm.Append(value, true);
			return algorithm.Hash;
		}

		public static byte[] CalculateHash(this HashAlgorithm algorithm, int value)
		{
			algorithm.Initialize();
			algorithm.Append(value, true);
			return algorithm.Hash;
		}

		public static byte[] CalculateHash(this HashAlgorithm algorithm, Type value)
		{
			algorithm.Initialize();
			algorithm.Append(value, true);
			return algorithm.Hash;
		}

		public static void Append(this HashAlgorithm algorithm, string value, bool isFinalAppend = false)
		{
			var bytes = Encoding.UTF8.GetBytes(value);
			algorithm.Append(bytes, isFinalAppend);
		}

		public static void Append(this HashAlgorithm algorithm, bool value, bool isFinalAppend = false)
		{
			var bytes = BitConverter.GetBytes(value);
			algorithm.Append(bytes, isFinalAppend);
		}

		public static void Append(this HashAlgorithm algorithm, int value, bool isFinalAppend = false)
		{
			var bytes = BitConverter.GetBytes(value);
			algorithm.Append(bytes, isFinalAppend);
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

			algorithm.Append((MemberInfo)value, isFinalAppend);
		}

		public static void Append(this HashAlgorithm algorithm, AssemblyName value, bool isFinalAppend = false)
		{
			algorithm.Append(value.FullName);
			algorithm.Append(value.Version.ToString(), isFinalAppend);
		}

		public static void Append(this HashAlgorithm algorithm, Attribute value, bool isFinalAppend = false)
		{
			algorithm.Append(value.GetType(), isFinalAppend);
		}

		public static void Append(this HashAlgorithm algorithm, PropertyInfo value, bool isFinalAppend = false)
		{
			algorithm.Append((MemberInfo)value);
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
			algorithm.Append((MemberInfo)value);
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

		public static void Append(this HashAlgorithm algorithm, MemberInfo value, bool isFinalAppend = false)
		{
			algorithm.Append((int)value.MemberType);
			if (value.DeclaringType != null)
			{
				algorithm.Append(value.DeclaringType);
			}
			int attributeCount = 0;
			foreach (var a in value.GetCustomAttributes<Attribute>())
			{
				algorithm.Append(a);
				++attributeCount;
			}
			algorithm.Append(attributeCount);
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
			if (value.Target == null)
			{
				algorithm.Append(ExplicitNull);
			}
			else
			{
				algorithm.Append(value.Target.GetType());
			}
			algorithm.Append(value.Method);

			var invocationList = value.GetInvocationList();
			foreach (var d in invocationList)
			{
				algorithm.Append(d);
			}
			algorithm.Append(invocationList.Length, isFinalAppend);
		}

		public static void Append(this HashAlgorithm algorithm, byte[] data, bool isFinalAppend = false)
		{
			if (isFinalAppend)
			{
				algorithm.TransformFinalBlock(data, 0, data.Length);
			}
			else
			{
				algorithm.TransformBlock(data, 0, data.Length, data, 0);
			}
		}
	}
}
