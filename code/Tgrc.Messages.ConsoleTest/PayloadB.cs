﻿using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Tgrc.Messages.ConsoleTest
{
	[MessagePackObject]
	public class PayloadB : IPayloadComponent
	{
		private static IPayloadComponentId id;

		[IgnoreMember]
		public IPayloadComponentId Id { get { return id; } }

		public static void SetId(IPayloadComponentId id)
		{
			PayloadB.id = id;
		}
	}
}
