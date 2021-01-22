using System;
using System.Security.Cryptography;
using Tgrc.Messages.Hash;

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
			algorithm.Append(Name);
			algorithm.Append(Type);
			algorithm.Append(Id.Id);
		}

		public byte[] Hash(HashAlgorithm algorithm)
		{
			algorithm.Initialize();

			algorithm.Append(Name);
			algorithm.Append(Type);
			algorithm.Append(Id.Id, true);
			return algorithm.Hash;
		}

		public override string ToString()
		{
			return string.Format("Name: {0} Type: {1} Id: {2}", Name, Type.FullName, Id.Id);
		}
	}
}
