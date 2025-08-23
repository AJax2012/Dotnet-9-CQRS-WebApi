#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace SourceName.Api.Loaders.JwtAuth;

public class BearerAuthorizationScheme
{
    public static string Key = "Authentication:Schemes:Bearer";
    
    [Required(AllowEmptyStrings = false)]
    public required string ValidIssuer { get; init; }
    
    [Required]
    [MinLength(1)]
    public List<string> ValidAudiences { get; init; } = [];
    
    [Required]
    [MinLength(1)]
    [ValidateEnumeratedItems]
    public List<BearerSigningKey> SigningKeys { get; init; } = [];
}
