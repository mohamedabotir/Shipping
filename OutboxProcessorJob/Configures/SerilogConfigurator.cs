using Serilog;
using Serilog.Sinks.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Infrastructure.Exceptions;

public static class SerilogConfigurator
{
    public static void Configure(IConfiguration configuration)
    {
        var elkLog = configuration.GetSection("ElkLog").Get<ElkLog>();

        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elkLog.ConnectionString))
            {
                AutoRegisterTemplate = true,
                IndexFormat = "logs-{0:yyyy.MM.dd}"
            })
            .CreateLogger();
    }
}
