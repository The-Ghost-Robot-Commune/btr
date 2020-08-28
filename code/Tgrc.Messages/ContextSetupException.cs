using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{

	[Serializable]
	public class ContextSetupException : Exception
	{
		public ContextSetupException() { }
		public ContextSetupException(string message) : base(message) { }
		public ContextSetupException(string message, Exception inner) : base(message, inner) { }
		protected ContextSetupException(
		 System.Runtime.Serialization.SerializationInfo info,
		 System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
