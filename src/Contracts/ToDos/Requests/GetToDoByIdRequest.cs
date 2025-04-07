using System.IdentityModel.Tokens.Jwt;

using FastEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace SourceName.Contracts.ToDos.Requests;

public class GetToDoByIdRequest
{
    [FromRoute(Name = "id")]
    public Guid Id { get; init; }

    [FromClaim(JwtRegisteredClaimNames.NameId, IsRequired = true)]
    public Guid UserId { get; init; }
}
