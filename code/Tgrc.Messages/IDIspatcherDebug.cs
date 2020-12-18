using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	/// <summary>
	/// All methods in this interface are for debugging purposes, they might be slow or overly complex
	/// </summary>
	public interface IDIspatcherDebug
	{

		/// <summary>
		/// Intended for debugging, might be very slow.
		/// </summary>
		/// <returns></returns>
		IEnumerable<Tuple<IPayloadComponentId, IEnumerable<IListener>>> GetAllListeners();
	}
}
