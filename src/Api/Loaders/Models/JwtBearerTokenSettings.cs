using Microsoft.AspNetCore.Authentication;

namespace SourceName.Api.Loaders.Models;

internal class JwtBearerTokenSettings : AuthenticationSchemeOptions
{
    public const string Key = "Auth:JwtBearerTokenSettings";
    public string SigningKey { get; init; } = null!;
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public int? ExpiryTimeInSeconds { get; init; }

    internal static JwtBearerTokenSettings GetJwtBearerTokenSettings(IConfiguration configuration)
    {
        var jwtBearerTokenSettings = configuration
            .GetRequiredSection(Key)
            .Get<JwtBearerTokenSettings>();
        
        ArgumentException.ThrowIfNullOrWhiteSpace(jwtBearerTokenSettings?.SigningKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(jwtBearerTokenSettings.Audience);
        ArgumentException.ThrowIfNullOrWhiteSpace(jwtBearerTokenSettings.Issuer);
        ArgumentNullException.ThrowIfNull(jwtBearerTokenSettings.ExpiryTimeInSeconds);
        
        return jwtBearerTokenSettings;
    }
}
