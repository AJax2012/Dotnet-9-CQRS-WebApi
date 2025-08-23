#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.ComponentModel.DataAnnotations;

namespace SourceName.Api.Loaders.JwtAuth;

public class BearerSigningKey
{
    [Required(AllowEmptyStrings = false)]
    public required string Id { get; init; }
    
    [Required(AllowEmptyStrings = false)]
    public required string Value { get; init; }
    
    [Required(AllowEmptyStrings = false)]
    public required string Issuer { get; init; }
    
    [Required]
    [Range(1, 256)]
    public int Length { get; init; }
}
