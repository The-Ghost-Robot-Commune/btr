﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	public interface IMessageProxy
	{
		string Name { get; }

		IContext Context { get; }

		void Send(IMessage message);
	}
}