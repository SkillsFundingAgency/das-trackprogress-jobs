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

    public ServiceBusTriggerNonAtomicEntryPoint(IFunctionEndpoint endpoint)
    {
        this.endpoint = endpoint;
    }

    //[FunctionName("TrackProcessJobs")]
    //public Task Run(
    //    [ServiceBusTrigger(QueueNames.TrackProgress, AutoCompleteMessages = false)] ServiceBusReceivedMessage message,
    //    ServiceBusClient client, ServiceBusMessageActions messageActions, ILogger logger, ExecutionContext context)
    //{
    //    logger.LogInformation("FullyQualifiedNameSpace {0}", client?.FullyQualifiedNamespace);
    //    return endpoint.ProcessAtomic(message, context, client, messageActions, logger);

    //}

    [FunctionName("TrackProcessJobsFQ")]
    public async Task Run(
        [ServiceBusTrigger(queueName: QueueNames.TrackProgress, Connection = "AzureWebJobsServiceBus__fullyQualifiedNamespace")] ServiceBusReceivedMessage message,
        ILogger logger,
        ExecutionContext context)
    {
        await endpoint.ProcessNonAtomic(message, context, logger);
    }

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

}