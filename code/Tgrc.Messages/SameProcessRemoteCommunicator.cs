using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	public class SameProcessRemoteCommunicator : IRemoteCommunicator
	{
		private Action<byte[]> receivers;

		public SameProcessRemoteCommunicator()
		{
			
		}

		public SameProcessRemoteCommunicator(SameProcessRemoteCommunicator partner)
		{
			this.Partner = partner;

			Partner.RegisterPartner(this);
		}

		public SameProcessRemoteCommunicator Partner { get; private set; }

		private void RegisterPartner(SameProcessRemoteCommunicator partner)
		{
			this.Partner = partner;
		}

		public void RegisterReceiver(Action<byte[]> receiver)
		{
			receivers += receiver;
		}

		public void Send(byte[] data)
		{
			Partner.Receive(data);
		}

		private void Receive(byte[] data)
		{
			receivers(data);
		}
	}
}
