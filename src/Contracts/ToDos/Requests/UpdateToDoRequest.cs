using System.IdentityModel.Tokens.Jwt;

using FastEndpoints;

using Microsoft.AspNetCore.Mvc;

namespace SourceName.Contracts.ToDos.Requests;

public class UpdateToDoRequest
{
    [FromRoute(Name = "id")]
    public Guid Id { get; init; }
    public required string Title { get; init; }
    public bool IsCompleted { get; init; }

    [FromClaim(JwtRegisteredClaimNames.NameId, IsRequired = true)]
    public Guid UserId { get; init; }
}
