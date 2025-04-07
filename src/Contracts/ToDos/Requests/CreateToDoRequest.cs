using System.IdentityModel.Tokens.Jwt;

using FastEndpoints;

namespace SourceName.Contracts.ToDos.Requests;

public class CreateToDoRequest
{
    public string? Title { get; init; }

    [FromClaim(JwtRegisteredClaimNames.NameId, IsRequired = true)]
    public Guid UserId { get; init; } 
}
