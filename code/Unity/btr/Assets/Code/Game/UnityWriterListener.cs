using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tgrc.Messages;
using UnityEngine;

namespace Tgrc.btr
{
	class UnityWriterListener : IListener
	{
		public void HandleMessage(IContext sender, IMessage message)
		{

			StringBuilder s = new StringBuilder();
			s.AppendLine(string.Format("Sender: {0}", sender));
			s.AppendLine(string.Format("PayloadCount: {0}", message.PayloadCount));
			int index = 0;
			foreach (var p in message.GetPayloadComponents())
			{
				s.AppendLine(string.Format("Payload {0}", index));
				s.AppendLine(p.ToString());

				++index;
			}

			Debug.Log(s.ToString());
		}
	}
}
