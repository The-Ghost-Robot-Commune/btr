using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Hash
{
	public static class PrimitiveTypesHashExtensions
	{
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

		public static byte[] CalculateHash(this HashAlgorithm algorithm, long value)
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

		public static void Append(this HashAlgorithm algorithm, long value, bool isFinalAppend = false)
		{
			var bytes = BitConverter.GetBytes(value);
			algorithm.Append(bytes, isFinalAppend);
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
