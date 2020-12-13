using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	/// <summary>
	/// All implementors must be serializable and be registered in the context in order to be usable. (Manually, or automatic via PayloadComponentAttribute)
	/// 
	/// </summary>
	public interface IPayloadComponent
	{
		IPayloadComponentId Id { get; }
	}
}
