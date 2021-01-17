using System;
using System.IO;

namespace Tgrc.Messages
{
	public delegate void Serialize(IPayloadComponent payload, MemoryStream stream);
	public delegate IPayloadComponent Deserialize(MemoryStream stream);

	public interface IPayloadDefinition
	{
		string Name { get; }
		Type Type { get; }
		Serialize Serializer { get; }
		Deserialize Deserializer { get; }
	}
}
