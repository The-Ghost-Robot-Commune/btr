using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Thread
{
	public class DotNetThreadStarter : IThreadStarter
	{
		public IThread CreateThread(ThreadStart startInfo)
		{
			return new Thread(startInfo);
		}

		private class Thread : IThread
		{
			private System.Threading.Thread internalThread;

			public Thread(ThreadStart startInfo)
			{
				this.StartInfo = startInfo;
			}

			public ThreadStart StartInfo { get; private set; }

			public bool IsRunning { get { return internalThread != null && internalThread.IsAlive; } }

			public void Dispose()
			{
				// Nothing needed
			}

			public void Start()
			{
				internalThread = new System.Threading.Thread(InternalThreadStart);
				internalThread.Start();
			}

			private void InternalThreadStart()
			{
				if (StartInfo.ParameterizedThread != null)
				{
					StartInfo.ParameterizedThread(StartInfo.Parameter);
				}
				else
				{
					StartInfo.Thread();
				}
			}

			public void Terminate()
			{
				internalThread.Abort();
			}
		}
	}
}
