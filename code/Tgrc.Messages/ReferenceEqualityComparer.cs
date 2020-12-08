using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	class ReferenceEqualityComparer<TClass> : IEqualityComparer<TClass>
		where TClass : class
	{
		public static readonly ReferenceEqualityComparer<TClass> Instance = new ReferenceEqualityComparer<TClass>();

		private ReferenceEqualityComparer()
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
