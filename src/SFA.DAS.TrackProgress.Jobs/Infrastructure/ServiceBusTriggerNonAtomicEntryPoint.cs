using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Logging;
using NServiceBus;

namespace SFA.DAS.TrackProgress.Jobs.Infrastructure;

internal class ServiceBusTriggerNonAtomicEntryPoint
{
    private readonly IFunctionEndpoint endpoint;
    private readonly ILogger _logger;

    public ServiceBusTriggerNonAtomicEntryPoint()
    {
    }


    //public ServiceBusTriggerNonAtomicEntryPoint(IFunctionEndpoint endpoint, ILogger logger)
    //{
    //    this.endpoint = endpoint;
    //    _logger = logger;
    //    _logger.LogInformation("ServiceBusTriggerNonAtomicEntryPoint constructor created");
    //}

    [FunctionName("TrackProcessJobsWithClient")]
    public Task Run(
        [ServiceBusTrigger(QueueNames.TrackProgress)] ServiceBusReceivedMessage message,
        ServiceBusClient client, ServiceBusMessageActions messageActions, ILogger logger, ExecutionContext context)
    {
        logger.LogInformation("FullyQualifiedNameSpace {0}", client?.FullyQualifiedNamespace);
        return Task.CompletedTask;
        //return endpoint.ProcessAtomic(message, context, client, messageActions, logger);

    }

    //[FunctionName("TrackProcessJobs")]
    //public async Task Run(
    //    [ServiceBusTrigger(queueName: QueueNames.TrackProgress, Connection = "AzureWebJobsServiceBus")] ServiceBusReceivedMessage message,
    //    ILogger logger,
    //    ExecutionContext context)
    //{
    //    await endpoint.ProcessNonAtomic(message, context, logger);
    //}

    //[FunctionName("TrackProcessJobsFQ")]
    //public async Task Run(
    //    [ServiceBusTrigger(queueName: QueueNames.TrackProgress, Connection = "AzureWebJobsServiceBus__fullyQualifiedNamespace")] ServiceBusReceivedMessage message,
    //    ILogger logger,
    //    ExecutionContext context)
    //{
    //    await endpoint.ProcessNonAtomic(message, context, logger);
    //}

    //[FunctionName("ProcessMessage")]
    //public async Task Run(
    //    // Setting AutoComplete to true (the default) processes the message non-transactionally
    //    [ServiceBusTrigger(QueueNames.TrackProgress, AutoCompleteMessages = true)]
    //    ServiceBusReceivedMessage message,
    //    ILogger logger,
    //    ExecutionContext executionContext)
    //{
    //    logger.LogInformation("Processing of message started");
    //    await endpoint.ProcessNonAtomic(message, executionContext, logger);
    //}

    //[FunctionName("ProcessMessageWithI")]
    //public Task Run([ServiceBusTrigger(QueueNames.TrackProgress, AutoCompleteMessages = false)] ServiceBusReceivedMessage message, IMyServiceBusClient client, ServiceBusMessageActions messageActions, ILogger logger, ExecutionContext executionContext)
    //{
    //    logger.LogInformation("IProcessed it");
    //    return Task.CompletedTask;
    //    //return endpoint.ProcessAtomic(message, executionContext, client, messageActions, logger);
    //}
}