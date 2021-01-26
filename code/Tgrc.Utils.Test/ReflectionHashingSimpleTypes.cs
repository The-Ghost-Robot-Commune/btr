using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Security.Cryptography;
using Tgrc.Hash;

namespace Tgrc.Utils.Test
{
	[TestClass]
	public class ReflectionHashingSimpleTypes
	{

		private HashAlgorithm CreateHash()
		{
			return SHA256.Create("SHA-256");
		}

		class Type1 { }

		class Type2 { }

		class Type3 : Type1 { }

		[TestMethod]
		public void DifferentTypeNames()
		{
			var calc = CreateHash();

			var hash1 = calc.CalculateHash(typeof(Type1));
			var hash2 = calc.CalculateHash(typeof(Type2));
			var hash3 = calc.CalculateHash(typeof(Type3));

			Assert.IsFalse(HashHelpers.IsEqual(hash1, hash2));
			Assert.IsFalse(HashHelpers.IsEqual(hash2, hash3));
			Assert.IsFalse(HashHelpers.IsEqual(hash3, hash1));
		}

		class Parent1
		{
			public class Type1 { }
		}
		class Parent2
		{
			public class Type1 { }
		}

		[TestMethod]
		public void NestedTypes()
		{
			var calc = CreateHash();

			var hash = calc.CalculateHash(typeof(Type1));
			var nestedHash1 = calc.CalculateHash(typeof(Parent1.Type1));
			var nestedHash2 = calc.CalculateHash(typeof(Parent2.Type1));

			Assert.IsFalse(HashHelpers.IsEqual(hash, nestedHash1));
			Assert.IsFalse(HashHelpers.IsEqual(nestedHash1, nestedHash2));
		}
	}
}
