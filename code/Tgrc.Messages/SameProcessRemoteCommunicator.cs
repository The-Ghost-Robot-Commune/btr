using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	public class SameProcessRemoteCommunicator : IRemoteCommunicator
	{
		public void RegisterReceiver(Action<byte[]> receiver)
		{
			throw new NotImplementedException();
		}

		public void Send(byte[] data)
		{
			throw new NotImplementedException();
		}
	}
}
