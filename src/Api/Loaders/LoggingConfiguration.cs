using Serilog;
using ILogger = Serilog.ILogger;

namespace SourceName.Api.Loaders;

internal static class LoggingConfiguration
{
    internal static ILogger AddLogging(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        
        ILogger logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateBootstrapLogger();

        builder.Host.UseSerilog(logger);
        return logger;
    }
}
