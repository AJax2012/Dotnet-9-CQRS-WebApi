using System.IdentityModel.Tokens.Jwt;

using FastEndpoints;

using Microsoft.AspNetCore.Mvc;

namespace SourceName.Api.ToDos.Update;

/// <summary>
/// Request to update an item by id.
/// </summary>
public class UpdateToDoRequest
{
    /// <summary>
    /// Id of to do
    /// </summary>
    [FromRoute(Name = "id")]
    public Guid Id { get; init; }
    
    /// <summary>
    /// Title of to do
    /// </summary>
    public required string Title { get; init; }
    
    /// <summary>
    /// Is the to do completed 
    /// </summary>
    public bool IsCompleted { get; init; }

    /// <summary>
    /// User Id from the auth token
    /// </summary>
    [FromClaim(JwtRegisteredClaimNames.NameId, IsRequired = true)]
    public Guid UserId { get; init; }
}
