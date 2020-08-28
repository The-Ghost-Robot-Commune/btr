﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	public interface IDispatcher
	{

		string Name { get; }

		IContext Context { get; }

		/// <summary>
		/// Sends all the messages that have arrived since the last dispatch
		/// </summary>
		void DispatchMessages();
	}
}
