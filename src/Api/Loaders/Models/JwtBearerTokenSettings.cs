namespace SourceName.Api.Loaders.Models;

internal class JwtBearerTokenSettings
{
    public const string Key = "Auth:JwtBearerTokenSettings";
    public string SigningKey { get; init; } = null!;
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public int? ExpiryTimeInSeconds { get; init; }
}
