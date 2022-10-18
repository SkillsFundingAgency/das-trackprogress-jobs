using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WireMock.Logging;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace SFA.DAS.TrackProgress.Jobs.MockServer;

public class OuterApiBuilder
{
    private static JsonSerializerSettings DefaultSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

    private readonly WireMockServer _server;

    public OuterApiBuilder(int port)
    {
        _server = WireMockServer.Start(new WireMockServerSettings
        {
            Port = port,
            UseSSL = true,
            StartAdminInterface = true,
            Logger = new WireMockConsoleLogger(),
        });
    }

    public static OuterApiBuilder Create(int port)
    {
        return new OuterApiBuilder(port);
    }

    public OuterApi Build()
    {
        return new OuterApi(_server);
    }

    public OuterApiBuilder WithNewProgress()
    {
        _server.Given(
                Request.Create()
                    .WithPath("/apprenticeships/*/snapshot")
                    .UsingPost()
                     )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(HttpStatusCode.OK));

        return this;
    }

    public OuterApiBuilder WithKsbs()
    {
        _server.Given(
                Request.Create()
                    .WithPath("/courses/*/populateksbs")
                    .UsingPost()
            )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(HttpStatusCode.OK));

        return this;
    }
}
