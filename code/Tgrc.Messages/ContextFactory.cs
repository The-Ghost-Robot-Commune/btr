using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tgrc.Log;

namespace Tgrc.Messages
{
	public class ContextFactory
	{
		public IContextSetup Create(string contextName, ILogger logger)
		{
			return Create(contextName, typeof(DefaultContext), logger);
		}

		public IContextSetup Create(string contextName, Type contextType, ILogger logger)
		{
			if (typeof(DefaultContext) == contextType)
			{
				return new DefaultContext.Setup(contextName, logger);
			}
			else
			{
				throw new ContextSetupException("Unsupported context type");
			}
		}
	}
}
