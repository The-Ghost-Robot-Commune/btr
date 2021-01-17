using System;
using System.Collections.Generic;

namespace Tgrc.Messages
{
	/// <summary>
	/// 
	/// Payloads and listener types are registered too a specific context, and can only be used within it. They can only be registred during setup in order to allow for pre-processing.
	/// </summary>
	public interface IContext
	{
		string Name { get; }

		IDispatcher Dispatcher { get; }
		IMessageComposer MessageComposer { get; }

		IPayloadComponentId FindPayloadId(string payloadName);

		string GetPayloadName(IPayloadComponentId id);
		Type GetPayloadType(IPayloadComponentId id);

		IPayloadDefinition GetPayloadDefinition(IPayloadComponentId id);
		IEnumerable<IPayloadComponentId> GetAllPayloadIds();
		IEnumerable<IPayloadDefinition> GetAllPayloadDefinitions();


	}
}
