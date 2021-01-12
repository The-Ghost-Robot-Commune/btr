using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroFormatter;

namespace Tgrc.Messages
{
	class ZeroFormatterImpl : ISerializer
	{
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

		public IMessage Deserialize(byte[] data)
		{
			throw new NotImplementedException();
		}

		public byte[] Serlialize(IMessage message)
		{
			byte[] bytePayloadCount = BitConverter.GetBytes(message.PayloadCount);
			List<byte> data = new List<byte>(bytePayloadCount.Length * (message.PayloadCount + 1));
			data.AddRange(bytePayloadCount);

			foreach (var payload in message.GetPayloadComponents())
			{
				var serializer = payloadDefinitions[payload.Id.Id].Serializer;

				byte[] payloadIdentifier = BitConverter.GetBytes(payload.Id.Id);
				byte[] payloadData = serializer(payload);

				data.AddRange(payloadIdentifier);
				data.AddRange(payloadData);
			}

			
		}
	}
}
