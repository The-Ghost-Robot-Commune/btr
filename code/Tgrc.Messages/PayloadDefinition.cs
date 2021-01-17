using System;

namespace Tgrc.Messages
{
	public class PayloadDefinition : IPayloadDefinition
	{

		public PayloadDefinition(string name, Type type, Serialize serializer, Deserialize deserializer)
		{
			this.Name = name;
			this.Type = type;
			this.Serializer = serializer;
			this.Deserializer = deserializer;
		}

		public string Name { get; private set; }
		public Type Type { get; private set; }

		public Serialize Serializer { get; private set; }
		public Deserialize Deserializer { get; private set; }

		public override string ToString()
		{
			return string.Format("Name: {0} Type: {1}", Name, Type.FullName);
		}
	}
}
