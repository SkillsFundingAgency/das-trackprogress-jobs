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

    //[FunctionName("TrackProcessJobsWithClient")]
    //public Task Run(
    //    [ServiceBusTrigger(QueueNames.TrackProgress)] ServiceBusReceivedMessage message,
    //    ServiceBusClient client, ServiceBusMessageActions messageActions, ILogger logger, ExecutionContext context)
    //{
    //    logger.LogInformation("FullyQualifiedNameSpace {0}", client?.FullyQualifiedNamespace);
    //    return endpoint.ProcessAtomic(message, context, client, messageActions, logger);

    //}

    [FunctionName("TrackProcessJobsEntryPoint")]
    public async Task Run(
        [ServiceBusTrigger(queueName: QueueNames.TrackProgress, Connection = "AzureWebJobsServiceBus")] ServiceBusReceivedMessage message,
        ILogger logger,
        ExecutionContext context)
    {
        await endpoint.ProcessNonAtomic(message, context, logger);
    }
}