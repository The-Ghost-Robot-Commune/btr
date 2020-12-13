using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	public class PayloadComponentIdComparer : IEqualityComparer<IPayloadComponentId>
	{
		public static readonly PayloadComponentIdComparer Instance = new PayloadComponentIdComparer();

		private PayloadComponentIdComparer() { }

		public bool Equals(IPayloadComponentId x, IPayloadComponentId y)
		{
			if (x == null && y == null)
			{
				return true;
			}
			if (x == null || y == null)
			{
				return false;
			}
			return x.Equals(y);
		}

		public int GetHashCode(IPayloadComponentId obj)
		{
			if (obj == null)
			{
				return 0;
			}

			return obj.GetHashCode();
		}
	}
}
