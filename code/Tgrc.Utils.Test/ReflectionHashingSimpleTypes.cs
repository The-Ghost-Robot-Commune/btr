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

		private class Type1 { }

		private class Type2 { }

		private class Type3 : Type1 { }

		[TestMethod]
		public void DifferentTypeNames()
		{
			var hash = CreateHash();
			
		}
	}
}
