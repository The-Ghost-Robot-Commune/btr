using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	class InstanceEqualityComparer<TClass> : IEqualityComparer<TClass>
		where TClass : class
	{
		public static readonly InstanceEqualityComparer<TClass> Instance = new InstanceEqualityComparer<TClass>();

		private InstanceEqualityComparer()
		{

		}

		public bool Equals(TClass x, TClass y)
		{
			return object.ReferenceEquals(x, y);
		}

		public int GetHashCode(TClass obj)
		{
			return obj == null ? 0 : obj.GetHashCode();
		}
	}
}
