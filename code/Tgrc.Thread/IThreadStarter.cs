﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Thread
{
	public interface IThreadStarter
	{
		IThread CreateThread(ThreadStart startInfo);
	}
}
