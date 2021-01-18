using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Thread
{
	public class ThreadStart
	{
		public ThreadStart(ThreadDelegate thread)
		{
			this.Thread = thread;
		}

		public ThreadStart(ParameterizedThreadDelegate thread, object parameter)
		{
			this.ParameterizedThread = thread;
			this.Parameter = parameter;
		}

		public ThreadDelegate Thread { get; private set; }
		public ParameterizedThreadDelegate ParameterizedThread { get; private set; }
		public object Parameter { get; private set; }
	}
}
