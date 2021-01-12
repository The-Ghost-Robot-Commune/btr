using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	class PayloadDefinition
	{
		public PayloadDefinition(string name, Type type, IPayloadComponentId id)
		{
			this.Name = name;
			this.Type = type;
			this.Id = id;
		}

		public string Name { get; private set; }
		public Type Type { get; private set; }
		public IPayloadComponentId Id { get; private set; }

		public override string ToString()
		{
			return string.Format("Name: {0} Type: {1} Id: {2}", Name, Type.FullName, Id.Id);
		}
	}
}
