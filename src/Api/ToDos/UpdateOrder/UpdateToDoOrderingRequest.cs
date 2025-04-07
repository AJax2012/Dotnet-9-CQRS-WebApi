using System.IdentityModel.Tokens.Jwt;

using FastEndpoints;

namespace SourceName.Api.ToDos.UpdateOrder;

/// <summary>
/// Request to update the display order of multiple to dos
/// </summary>
public class UpdateToDoOrderingRequest
{
    /// <summary>
    /// Id and display order of to dos to update
    /// </summary>
    public Dictionary<Guid, int> ToDos { get; init; } = new();
    
    /// <summary>
    /// User id from the auth token
    /// </summary>
    [FromClaim(JwtRegisteredClaimNames.NameId, IsRequired = true)]
    public Guid UserId { get; init; }
};
