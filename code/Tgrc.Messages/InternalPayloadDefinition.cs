using System;

namespace Tgrc.Messages
{
	class InternalPayloadDefinition : IPayloadDefinition
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


		public override string ToString()
		{
			return string.Format("Name: {0} Type: {1} Id: {2}", Name, Type.FullName, Id.Id);
		}
	}
}
