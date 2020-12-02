using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{

	public interface IPayloadComponentId : IEquatable<IPayloadComponentId>
	{
		int Id { get; }
	}
}
