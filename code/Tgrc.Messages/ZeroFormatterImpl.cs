using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroFormatter;

namespace Tgrc.Messages
{
	class ZeroFormatterImpl : ISerializer
	{
		private const int intByteSize = 4;

		private readonly PayloadDefinition[] payloadDefinitions;

		public ZeroFormatterImpl(List<PayloadDefinition> payloadTypes, IMessageComposer composer)
		{
			payloadDefinitions = new PayloadDefinition[payloadTypes.Count];
			for (int i = 0; i < payloadDefinitions.Length; i++)
			{
				payloadDefinitions[i] = payloadTypes[i];
			}

			this.MessageComposer = composer;
		}

		public IMessageComposer MessageComposer { get; private set; }

		public IMessage Deserialize(MemoryStream stream)
		{
			byte[] intBuffer = new byte[intByteSize];
			stream.Read(intBuffer, 0, intBuffer.Length);

			int numberOfPayloads = BitConverter.ToInt32(intBuffer, 0);
			List<IPayloadComponent> payloadComponents = new List<IPayloadComponent>(numberOfPayloads);
			for (int i = 0; i < numberOfPayloads; i++)
			{
				stream.Read(intBuffer, 0, intBuffer.Length);
				int payloadId = BitConverter.ToInt32(intBuffer, 0);

				var deserializer = payloadDefinitions[payloadId].Deserializer;
				IPayloadComponent component = deserializer(stream);
				payloadComponents.Add(component);
			}

			return MessageComposer.Compose(payloadComponents);
		}

		public void Serialize(IMessage message, MemoryStream stream)
		{
			byte[] bytePayloadCount = BitConverter.GetBytes(message.PayloadCount);

			Debug.Assert(bytePayloadCount.Length == intByteSize);

			stream.Write(bytePayloadCount, 0, bytePayloadCount.Length);

			foreach (var payload in message.GetPayloadComponents())
			{
				int payloadId = payload.Id.Id;

				byte[] payloadIdentifier = BitConverter.GetBytes(payloadId);
				Debug.Assert(payloadIdentifier.Length == intByteSize);

				stream.Write(payloadIdentifier, 0, payloadIdentifier.Length);

				var serializer = payloadDefinitions[payloadId].Serializer;
				serializer(payload, stream);
			}
		}
	}
}
