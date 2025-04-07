using System.IdentityModel.Tokens.Jwt;

using FastEndpoints;

using Microsoft.AspNetCore.Mvc;

namespace SourceName.Api.ToDos.GetById;

/// <summary>
/// Request to get an item by id
/// </summary>
public class GetToDoByIdRequest
{
    /// <summary>
    /// Id of the to do to get
    /// </summary>
    [FromRoute(Name = "id")]
    public Guid Id { get; init; }

    /// <summary>
    /// User Id from auth token
    /// </summary>
    [FromClaim(JwtRegisteredClaimNames.NameId, IsRequired = true)]
    public Guid UserId { get; init; }
}
