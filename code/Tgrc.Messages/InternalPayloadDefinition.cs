using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Tgrc.Messages.PayloadDefinition;

namespace Tgrc.Messages
{
	class InternalPayloadDefinition
	{
		private PayloadDefinition baseDefinition;

		public InternalPayloadDefinition(PayloadDefinition baseDefinition, IPayloadComponentId id)
		{
			this.baseDefinition = baseDefinition;
			this.Id = id;
		}

		public string Name { get { return baseDefinition.Name; } }
		public Type Type { get { return baseDefinition.Type; } }

		public Serialize Serializer { get { return baseDefinition.Serializer; } }
		public Deserialize Deserializer { get { return baseDefinition.Deserializer; } }
		public IPayloadComponentId Id { get; private set; }


		public override string ToString()
		{
			return string.Format("{0} Id {1}", baseDefinition, Id.Id);
		}
	}
}
