using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tgrc.Hash;

namespace Tgrc.Utils.Test
{
	[TestClass]
	public class HashingExtensionFunctionality
	{
		private HashAlgorithm CreateHash()
		{
			return SHA256.Create("SHA-256");
		}

		[TestMethod]
		public void ReusingAlgorithm()
		{
			int value1 = 5;
			int value2 = 15;
			byte[] hash1 = CreateHash().CalculateHash(value1);
			byte[] hash2 = CreateHash().CalculateHash(value2);
			
			var reusingHash = CreateHash();
			byte[] rehash1 = reusingHash.CalculateHash(value1);
			Assert.IsTrue(HashHelpers.IsEqual(hash1, rehash1));

			byte[] rehash2 = reusingHash.CalculateHash(value2);
			Assert.IsTrue(HashHelpers.IsEqual(hash2, rehash2));
		}

		[TestMethod]
		public void ReusingAlgorithm_Reorder()
		{
			int value1 = 5;
			int value2 = 15;
			byte[] hash1 = CreateHash().CalculateHash(value1);
			byte[] hash2 = CreateHash().CalculateHash(value2);

			var reusingHash = CreateHash();
			byte[] rehash2 = reusingHash.CalculateHash(value2);
			Assert.IsTrue(HashHelpers.IsEqual(hash2, rehash2));

			byte[] rehash1 = reusingHash.CalculateHash(value1);
			Assert.IsTrue(HashHelpers.IsEqual(hash1, rehash1));
		}
	}
}
