using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Reflection;

namespace BuildingBlocks.Logging;
public static class SeriLogger
{
    public static Action<HostBuilderContext, LoggerConfiguration> Configure =>
    (context, configuration) =>
    {
        configuration
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{Application} {Environment}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
        .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
        .ReadFrom.Configuration(context.Configuration);

        var elasticSearchConnectionString = context.Configuration.GetConnectionString("Elasticsearch");

        if (!string.IsNullOrEmpty(elasticSearchConnectionString))
        {
            configuration.WriteTo.Elasticsearch(new(new Uri(elasticSearchConnectionString))
            {
                IndexFormat = $"applogs-{Assembly.GetEntryAssembly()!.GetName().Name!.ToLower().Replace(".", "-")}-{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-logs-{DateTime.UtcNow:yyyy-MM}",
                AutoRegisterTemplate = true,
                NumberOfShards = 2,
                NumberOfReplicas = 1
            });
        }
    };
}
