using System.IdentityModel.Tokens.Jwt;

using FastEndpoints;

namespace SourceName.Contracts.ToDos.Requests;

public class UpdateToDoOrderingRequest
{
    public Dictionary<Guid, int> ToDos { get; init; } = new();
    
    [FromClaim(JwtRegisteredClaimNames.NameId, IsRequired = true)]
    public Guid UserId { get; init; }
};
