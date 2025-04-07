using System.IdentityModel.Tokens.Jwt;

using FastEndpoints;

using Microsoft.AspNetCore.Mvc;

namespace SourceName.Api.ToDos.Delete;

/// <summary>
/// Request for deleting a To Do
/// </summary>
public class DeleteToDoRequest
{
    /// <summary>
    /// Id of to do
    /// </summary>
    [FromRoute(Name = "id")]
    public Guid Id { get; init; }

    /// <summary>
    /// User Id from auth token
    /// </summary>
    [FromClaim(JwtRegisteredClaimNames.NameId, IsRequired = true)]
    public Guid UserId { get; init; }
}
