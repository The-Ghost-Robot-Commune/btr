﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	/// <summary>
	/// All implementations need to be serializable in some way
	/// 
	/// </summary>
	public interface IPayload
	{
		IPayloadId Id { get; }

	}
}
