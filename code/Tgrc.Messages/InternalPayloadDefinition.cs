using System;
using System.Security.Cryptography;
using Tgrc.Hash;

namespace Tgrc.Messages
{
	class InternalPayloadDefinition : IPayloadDefinition, IHashable
	{
		public InternalPayloadDefinition(IPayloadDefinition baseDefinition, IPayloadComponentId id)
		{
			this.Name = baseDefinition.Name;
			this.Type = baseDefinition.Type;
			this.Serializer = baseDefinition.Serializer;
			this.Deserializer = baseDefinition.Deserializer;
			this.Id = id;
		}

		public string Name { get; private set; }
		public Type Type { get; private set; }
		public Serialize Serializer { get; private set; }
		public Deserialize Deserializer { get; private set; }
		public IPayloadComponentId Id { get; private set; }

		public void IncrementalHash(HashAlgorithm algorithm)
		{
			Hash(algorithm, false);
		}

		public byte[] Hash(HashAlgorithm algorithm)
		{
			algorithm.Initialize();

			Hash(algorithm, true);
			return algorithm.Hash;
		}

		private void Hash(HashAlgorithm algorithm, bool isFinalAppend)
		{
			algorithm.Append(Name);
			algorithm.AppendType(Type);
			// TODO Find a way to include the serializer and deserializer in the identity hash
			//algorithm.Append(Serializer);
			//algorithm.Append(Deserializer);
			algorithm.Append(Id.Id, isFinalAppend);
		}

		public override string ToString()
		{
			return string.Format("Name: {0} Type: {1} Id: {2}", Name, Type.FullName, Id.Id);
		}
	}
}
