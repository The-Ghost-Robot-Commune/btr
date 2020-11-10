using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	[System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	public sealed class ListenerMethodAttribute : Attribute
	{
		public ListenerMethodAttribute()
		{

		}
	}
}
