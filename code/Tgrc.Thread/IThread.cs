﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Thread
{
	public interface IThread : IDisposable
	{
		ThreadStart StartInfo { get; }
		bool IsRunning { get; }

		void Start();

		void Terminate();
	}
}
