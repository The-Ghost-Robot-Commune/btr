using System.Security.Cryptography;

namespace Tgrc.Messages
{
	public interface IHashable
	{
		/// <summary>
		/// Computes the hash of the current object using the specified algorithm
		/// </summary>
		/// <param name="algorithm"></param>
		/// <returns></returns>
		byte[] Hash(HashAlgorithm algorithm);

		/// <summary>
		/// "Add the current object" to a bigger hash value that is calculated over multiple objects.
		/// </summary>
		/// <param name="algorithm"></param>
		void IncrementalHash(HashAlgorithm algorithm);
		
	}
}
