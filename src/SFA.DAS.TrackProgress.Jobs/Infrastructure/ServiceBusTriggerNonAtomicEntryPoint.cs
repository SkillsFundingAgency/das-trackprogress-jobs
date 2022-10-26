﻿using System.Threading.Tasks;
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

    [FunctionName("TrackProcessJobs")]
    public async Task Run([ServiceBusTrigger(QueueNames.TrackProgress, AutoCompleteMessages = false)] ServiceBusReceivedMessage message, ServiceBusClient client, ServiceBusMessageActions messageActions, ILogger logger, ExecutionContext context)

    //public async Task Run(
    //    [ServiceBusTrigger(queueName: QueueNames.TrackProgress, Connection = "AzureWebJobsServiceBus")] ServiceBusReceivedMessage message,
    //    ILogger logger,
    //    ExecutionContext context)
    {
        await endpoint.ProcessNonAtomic(message, context, logger);
    }
}