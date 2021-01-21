using System;
using System.Security.Cryptography;
using System.Text;

namespace Tgrc.Messages.Hash
{
	public static class HashExtensions
	{
		public static byte[] CalculateHash(this HashAlgorithm algorithm, string value)
		{
			var bytes = Encoding.UTF8.GetBytes(value);
			algorithm.Initialize();
			return algorithm.ComputeHash(bytes);
		}

		public static byte[] CalculateHash(this HashAlgorithm algorithm, bool value)
		{
			var bytes = BitConverter.GetBytes(value);
			algorithm.Initialize();
			return algorithm.ComputeHash(bytes);
		}

		public static byte[] CalculateHash(this HashAlgorithm algorithm, int value)
		{
			var bytes = BitConverter.GetBytes(value);
			algorithm.Initialize();
			return algorithm.ComputeHash(bytes);
		}

		public static byte[] CalculateHash(this HashAlgorithm algorithm, Type value)
		{
			algorithm.Initialize();
			algorithm.AppendIncrementalValue(value.Assembly.FullName);
			return algorithm.AppendIncrementalAndHash(value.FullName);
		}

		public static void AppendIncrementalValue(this HashAlgorithm algorithm, string value)
		{
			var bytes = Encoding.UTF8.GetBytes(value);
			algorithm.TransformBlock(bytes, 0, bytes.Length, bytes, 0);
		}

		public static void AppendIncrementalValue(this HashAlgorithm algorithm, bool value)
		{
			var bytes = BitConverter.GetBytes(value);
			algorithm.TransformBlock(bytes, 0, bytes.Length, bytes, 0);
		}

		public static void AppendIncrementalValue(this HashAlgorithm algorithm, int value)
		{
			var bytes = BitConverter.GetBytes(value);
			algorithm.TransformBlock(bytes, 0, bytes.Length, bytes, 0);
		}

		public static void AppendIncrementalValue(this HashAlgorithm algorithm, Type value)
		{
			algorithm.AppendIncrementalValue(value.Assembly.FullName);
			algorithm.AppendIncrementalValue(value.FullName);
		}

		public static byte[] AppendIncrementalAndHash(this HashAlgorithm algorithm, string value)
		{
			var bytes = Encoding.UTF8.GetBytes(value);
			algorithm.TransformFinalBlock(bytes, 0, bytes.Length);
			return algorithm.Hash;
		}

		public static byte[] AppendIncrementalAndHash(this HashAlgorithm algorithm, bool value)
		{
			var bytes = BitConverter.GetBytes(value);
			algorithm.TransformFinalBlock(bytes, 0, bytes.Length);
			return algorithm.Hash;
		}
		public static byte[] AppendIncrementalAndHash(this HashAlgorithm algorithm, int value)
		{
			var bytes = BitConverter.GetBytes(value);
			algorithm.TransformFinalBlock(bytes, 0, bytes.Length);
			return algorithm.Hash;
		}
		public static byte[] AppendIncrementalAndHash(this HashAlgorithm algorithm, Type value)
		{
			algorithm.AppendIncrementalValue(value.Assembly.FullName);
			return algorithm.AppendIncrementalAndHash(value.FullName);
		}
	}
}
