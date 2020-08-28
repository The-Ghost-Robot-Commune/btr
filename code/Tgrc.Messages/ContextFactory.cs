using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgrc.Messages
{
	public class ContextFactory
	{
		public IContextSetup Create(string contextName)
		{
			return Create(contextName, typeof(DefaultContext));
		}

		public IContextSetup Create(string contextName, Type contextType)
		{
			if (typeof(DefaultContext) == contextType)
			{
				return new DefaultContext.Setup(contextName);
			}
			else
			{
				throw new ContextSetupException("Unsupported context type");
			}
		}
	}
}
