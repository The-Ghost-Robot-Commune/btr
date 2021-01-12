using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	public class PayloadDefinition
	{
		public delegate void Serialize(IPayloadComponent payload, MemoryStream stream);
		public delegate IPayloadComponent Deserialize(MemoryStream stream);

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
