using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	public interface ISerializer
	{
		void Serialize(IMessage message, MemoryStream stream);

		IMessage Deserialize(MemoryStream stream);
	}
}
