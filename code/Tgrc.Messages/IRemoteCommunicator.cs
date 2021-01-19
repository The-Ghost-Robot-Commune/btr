using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	public interface IRemoteCommunicator : IDisposable
	{
		/// <summary>
		/// Send data to the remote target
		/// </summary>
		/// <param name="data"></param>
		void Send(MemoryStream data);

		/// <summary>
		/// Register a receiver method that will get called when new data arrives. 
		/// NOTE: The receiver will get called from another thread.
		/// </summary>
		/// <param name="receiver"></param>
		void RegisterReceiver(Action<MemoryStream> receiver);
	}
}
