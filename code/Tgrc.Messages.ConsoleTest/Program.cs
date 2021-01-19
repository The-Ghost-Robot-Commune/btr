﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tgrc.Thread;

namespace Tgrc.Messages.ConsoleTest
{
	static class Program
	{
		static void Main(string[] args)
		{
			MemoryMappedTest(args);


			//SameProcessRemoteCommunicatorTest();

		}

		private static void MemoryMappedTest(string[] args)
		{
			bool isClient = args.Length > 0 && args[0] == "client";

			string contextName = "Context" + (isClient ? ".Client" : ".Host");
			IContext context = CreateContext(contextName);


			DotNetThreadStarter threadStarter = new DotNetThreadStarter();
			MemoryMappedCommunicator communicator = new MemoryMappedCommunicator(threadStarter, "Tgrc.Messages.MemoryMap", !isClient);

			RemoteDispatcherProxy proxy = CreateRemoteDispatcher(context, communicator);

			communicator.StartThreads();

			IMessage message = CreateBasicMessage(context);

			context.Dispatcher.Send(message);

			while (true)
			{
				context.Dispatcher.DispatchMessages();
				proxy.ForwardRemoteMessages(); 
			}
		}

		private static IMessage CreateBasicMessage(IContext context)
		{
			PayloadA plA = new PayloadA();
			PayloadB plB = new PayloadB();

			return context.MessageComposer.Compose(plA, plB);
		}

		private static void SameProcessRemoteCommunicatorTest()
		{
			IContext contextA = CreateContext("ContextA");
			IContext contextB = CreateContext("ContextB");
			
			SameProcessRemoteCommunicator communicatorA = new SameProcessRemoteCommunicator();
			SameProcessRemoteCommunicator communicatorB = new SameProcessRemoteCommunicator(communicatorA);

			RemoteDispatcherProxy proxyA = CreateRemoteDispatcher(contextA, communicatorA);
			RemoteDispatcherProxy proxyB = CreateRemoteDispatcher(contextB, communicatorB);

			PayloadA plA = new PayloadA();
			PayloadB plB = new PayloadB();

			var message = contextA.MessageComposer.Compose(plA, plB);
			contextA.Dispatcher.Send(message);
			contextA.Dispatcher.DispatchMessages();

			proxyB.ForwardRemoteMessages();
			contextB.Dispatcher.DispatchMessages();
		}

		private static RemoteDispatcherProxy CreateRemoteDispatcher(IContext context, IRemoteCommunicator communicator)
		{
			RemoteDispatcherProxy proxy = new RemoteDispatcherProxy(context.Dispatcher, context.Serializer, communicator);
			WriterListener listener = new WriterListener();
			context.Dispatcher.RegisterListenerForAll(listener);
			return proxy;
		}

		private static void SimpleContextListener()
		{
			IContext context = CreateContext("TestContext");

			IDispatcher dispatcher = context.Dispatcher;
			IMessageComposer composer = context.MessageComposer;

			ListenerA listener = new ListenerA();

			dispatcher.RegisterListenerForAll(listener);


			PayloadA plA = new PayloadA();
			PayloadB plB = new PayloadB();

			var message = composer.Compose(plA, plB);
			dispatcher.Send(message);
			dispatcher.DispatchMessages();
		}

		private static IContext CreateContext(string contextName)
		{
			ContextFactory factory = new ContextFactory();
			var contextSetup = factory.Create(contextName, null);

			List<IPayloadComponentId> payloadIds = new List<IPayloadComponentId>();

			Assembly currentAssembly = Assembly.GetExecutingAssembly();
			var payloads = ContextUtilities.FindPayloadComponents(currentAssembly);
			foreach (var payload in payloads)
			{
				var id = contextSetup.RegisterPayloadComponent(payload);
				payloadIds.Add(id);

				if (payload.Name == nameof(PayloadA))
				{
					PayloadA.SetId(id);
				}
			}


			PayloadDefinition payloadBDefinition = new PayloadDefinition(nameof(PayloadB), typeof(PayloadB), ContextUtilities.FindSerializeMethod<PayloadB>(), ContextUtilities.FindDeserializeMethod<PayloadB>());
			var bId = contextSetup.RegisterPayloadComponent(payloadBDefinition);
			payloadIds.Add(bId);
			PayloadB.SetId(bId);
			return contextSetup.EndSetup();
		}
		
	}
}
