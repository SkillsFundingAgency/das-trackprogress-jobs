//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using NServiceBus.Pipeline;

//namespace SFA.DAS.NServiceBus.Lite
//{
//    internal class LogIncomingBehaviour
//        : IBehavior<IIncomingLogicalMessageContext, IIncomingLogicalMessageContext>
//    {
//        private readonly ILogger _logger;

//        public LogIncomingBehaviour()
//        {
//            var _logger = LoggerFactory.Create(b => b.Services.AddLogging()).CreateLogger<LogIncomingBehaviour>();
//            _logger = new Logger<LogIncomingBehaviour>();
//            _logger = LoggerFactory.AddApplicationInsights.Create(b=>b.Services.GetRequiredService())
//        }

//        public async Task Invoke(IIncomingLogicalMessageContext context, Func<IIncomingLogicalMessageContext, Task> next)
//        {
//            context.MessageHeaders.TryGetValue("NServiceBus.MessageIntent", out var intent);
//            var types = context.Message.MessageType.Name;
//            //_logger.LogInformation($"Received message {context.MessageId} (`{types}` intent `{intent}`)");

//            await next(context);
//        }
//    }

//    internal class LogOutgoingBehaviour
//        : IBehavior<IOutgoingLogicalMessageContext, IOutgoingLogicalMessageContext>
//    {
//        private readonly ILogger _logger;

//        public LogOutgoingBehaviour()
//        {
//            //_logger = LoggerFactory.Create(b => b.ConfigureLogging()).CreateLogger<LogOutgoingBehaviour>();
//        }

//        public async Task Invoke(IOutgoingLogicalMessageContext context, Func<IOutgoingLogicalMessageContext, Task> next)
//        {
//            var types = context.Message.MessageType.Name;
//            //_logger.LogInformation($"Sending message {context.MessageId} (`{types}`)");

//            await next(context);
//        }
//    }
//}
