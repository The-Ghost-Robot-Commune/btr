using System;
using System.Security.Cryptography;
using System.Text;

namespace Tgrc.Messages.Hash
{
	public static class HashExtensions
	{
		public static byte[] CalculateHash(this HashAlgorithm algorithm, string value)
		{
			algorithm.Initialize();
			algorithm.AppendIncrementalValue(value, true);
			return algorithm.Hash;
		}

		public static byte[] CalculateHash(this HashAlgorithm algorithm, bool value)
		{
			algorithm.Initialize();
			algorithm.AppendIncrementalValue(value, true);
			return algorithm.Hash;
		}

		public static byte[] CalculateHash(this HashAlgorithm algorithm, int value)
		{
			algorithm.Initialize();
			algorithm.AppendIncrementalValue(value, true);
			return algorithm.Hash;
		}

		public static byte[] CalculateHash(this HashAlgorithm algorithm, Type value)
		{
			algorithm.Initialize();
			algorithm.AppendIncrementalValue(value, true);
			return algorithm.Hash;
		}

		public static void AppendIncrementalValue(this HashAlgorithm algorithm, string value, bool isFinalAppend = false)
		{
			var bytes = Encoding.UTF8.GetBytes(value);
			algorithm.Append(bytes, isFinalAppend);
		}

		public static void AppendIncrementalValue(this HashAlgorithm algorithm, bool value, bool isFinalAppend = false)
		{
			var bytes = BitConverter.GetBytes(value);
			algorithm.Append(bytes, isFinalAppend);
		}

		public static void AppendIncrementalValue(this HashAlgorithm algorithm, int value, bool isFinalAppend = false)
		{
			var bytes = BitConverter.GetBytes(value);
			algorithm.Append(bytes, isFinalAppend);
		}

		public static void AppendIncrementalValue(this HashAlgorithm algorithm, Type value, bool isFinalAppend = false)
		{
			algorithm.AppendIncrementalValue(value.Assembly.FullName);
			algorithm.AppendIncrementalValue(value.FullName, isFinalAppend);
		}

		private static void Append(this HashAlgorithm algorithm, byte[] data, bool isFinalAppend)
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
