using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	[System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
	public sealed class PayloadComponentAttribute : Attribute
	{
		public PayloadComponentAttribute(string name)
		{
			this.Name = name;
		}

		public string Name { get; private set; }
	}

}
