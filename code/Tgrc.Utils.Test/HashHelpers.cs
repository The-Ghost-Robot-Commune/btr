using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Utils.Test
{
	class HashHelpers
	{
		public static bool IsEqual(byte[] hashA, byte[] hashB)
		{
			if (hashA.Length != hashB.Length)
			{
				return false;
			}

			for (int i = 0; i < hashA.Length; i++)
			{
				if (hashA[i] != hashB[i])
				{
					return false;
				}
			}

			return true;
		}

		public static string ToString(byte[] hash)
		{
			StringBuilder str = new StringBuilder(64);
			foreach (byte hashByte in hash)
			{
				str.Append(hashByte.ToString("x2"));
			}
			return str.ToString();
		}
	}
}
