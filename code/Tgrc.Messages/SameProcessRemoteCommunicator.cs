using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	public class SameProcessRemoteCommunicator : IRemoteCommunicator
	{
		private Action<MemoryStream> receivers;

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

		public void RegisterReceiver(Action<MemoryStream> receiver)
		{
			receivers += receiver;
		}

		public void Send(MemoryStream data)
		{
			Partner.Receive(data);
		}

		private void Receive(MemoryStream data)
		{
			receivers(data);
		}

		public void Dispose()
		{
			// Nothing needed here
		}
	}
}
