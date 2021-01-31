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

			var hash1 = calc.CalculateClassHash(typeof(Type1));
			var hash2 = calc.CalculateClassHash(typeof(Type2));
			var hash3 = calc.CalculateClassHash(typeof(Type3));

			Assert.IsFalse(HashHelpers.IsEqual(hash1, hash2));
			Assert.IsFalse(HashHelpers.IsEqual(hash2, hash3));
			Assert.IsFalse(HashHelpers.IsEqual(hash3, hash1));
		}

		class Parent1
		{
			public class Type1 { }
			public class Type2 { }
			public class Type3 : Type1 { }
		}
		class Parent2
		{
			public class Type1 { }
			public class Type2 { }
			public class Type3 : Type1 { }
		}


		[TestMethod]
		public void NestedTypes_SameNames()
		{
			var calc = CreateHash();

			var hash = calc.CalculateClassHash(typeof(Type1));
			var nestedHash1 = calc.CalculateClassHash(typeof(Parent1.Type1));
			var nestedHash2 = calc.CalculateClassHash(typeof(Parent2.Type1));

			// As all the hashed types are empty and have the same name, they should hash as equal
			Assert.IsTrue(HashHelpers.IsEqual(hash, nestedHash1));
			Assert.IsTrue(HashHelpers.IsEqual(nestedHash1, nestedHash2));
		}

		[TestMethod]
		public void NestedTypes_DifferentNames()
		{
			var calc = CreateHash();

			var hash = calc.CalculateClassHash(typeof(Type1));
			var nestedHash1 = calc.CalculateClassHash(typeof(Parent1.Type2));
			var nestedHash2 = calc.CalculateClassHash(typeof(Parent2.Type3));

			Assert.IsFalse(HashHelpers.IsEqual(hash, nestedHash1));
			Assert.IsFalse(HashHelpers.IsEqual(nestedHash1, nestedHash2));
		}

		class SameNamesDifferentSignaturesA
		{
			public class Property_DifferentAccess1
			{
				public int Value { get; }
			}
			public class Property_DifferentAccess2
			{
				public int Value { get; }
			}
			public class Property_DifferentType1
			{
				public int Value { get; set; }
			}
			public class PropertyMethod1
			{
				public int Value { get; }
			}
		}

		class SameNamesDifferentSignaturesB
		{
			public class Property_DifferentAccess1
			{
				public int Value { get; set; }
			}
			public class Property_DifferentAccess2
			{
				public int Value { set { } }
			}
			public class Property_DifferentType1
			{
				public long Value { get; set; }
			}
			public class PropertyMethod1
			{
				public int Value()
				{
					return 0;
				}
			}
		}

		[TestMethod]
		public void SameNamesDifferentSignatures1()
		{
			var hash1 = CreateHash().CalculateClassHash(typeof(SameNamesDifferentSignaturesA.Property_DifferentAccess1));
			var hash2 = CreateHash().CalculateClassHash(typeof(SameNamesDifferentSignaturesB.Property_DifferentAccess1));

			Assert.IsFalse(HashHelpers.IsEqual(hash1, hash2));
		}
		[TestMethod]
		public void SameNamesDifferentSignatures2()
		{
			var hash1 = CreateHash().CalculateClassHash(typeof(SameNamesDifferentSignaturesA.Property_DifferentAccess2));
			var hash2 = CreateHash().CalculateClassHash(typeof(SameNamesDifferentSignaturesB.Property_DifferentAccess2));

			Assert.IsFalse(HashHelpers.IsEqual(hash1, hash2));
		}
		[TestMethod]
		public void SameNamesDifferentSignatures3()
		{
			var hash1 = CreateHash().CalculateClassHash(typeof(SameNamesDifferentSignaturesA.Property_DifferentType1));
			var hash2 = CreateHash().CalculateClassHash(typeof(SameNamesDifferentSignaturesB.Property_DifferentType1));

			Assert.IsFalse(HashHelpers.IsEqual(hash1, hash2));
		}
		[TestMethod]
		public void SameNameDifferentTypesOfMethods()
		{
			var hash1 = CreateHash().CalculateClassHash(typeof(SameNamesDifferentSignaturesA.PropertyMethod1));
			var hash2 = CreateHash().CalculateClassHash(typeof(SameNamesDifferentSignaturesB.PropertyMethod1));

			Assert.IsFalse(HashHelpers.IsEqual(hash1, hash2));
		}
	}
}
