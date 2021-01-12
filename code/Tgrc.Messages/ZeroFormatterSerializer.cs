using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	public class ZeroFormatterSerializer : ISerializer
	{
		public IMessage Deserialize(byte[] data)
		{
			throw new NotImplementedException();
		}

		public byte[] Serlialize(IMessage message)
		{
			throw new NotImplementedException();
		}
	}
}
