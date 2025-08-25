using System.Runtime.CompilerServices;

namespace SourceName.Test.Integration.VerifyConfig;

public static class VerifyGlobalSettings
{
    [ModuleInitializer]
    public static void Initialize()
    { 
        VerifyHttp.Initialize();
        Recording.Start();
    }

    internal static VerifySettings GetSettings()
    {
        var settings = new VerifySettings();
        settings.UseDirectory("Snapshots");
        settings.ScrubInlineGuids();
        settings.ScrubMembers("Correlation-Id", "traceId");
        return settings;
    }
}
