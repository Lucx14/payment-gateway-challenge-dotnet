using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

using WireMock.Server;

namespace PaymentGateway.Integration.Tests.TestSetup;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public WireMockServer WireMockServer { get; } = WireMockServer.Start();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddJsonFile("appsettings.Test.json", true, true);

                var inMemorySettings = new Dictionary<string, string?>
                {
                    { "BankSimulatorApi:BaseAddress", WireMockServer.Urls[0] }
                };

                config.AddInMemoryCollection(inMemorySettings);
            });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            WireMockServer?.Stop();
            WireMockServer?.Dispose();
        }

        base.Dispose(disposing);

    }
}