using System.IdentityModel.Tokens.Jwt;

using FastEndpoints;

namespace SourceName.Api.ToDos.Create;

/// <summary>
/// Creates a To Do
/// </summary>
public class CreateToDoRequest
{
    /// <summary>
    /// To do toDoTitle
    /// </summary>
    public string? Title { get; init; }

    /// <summary>
    /// User Id from authentication token.
    /// </summary>
    [FromClaim(JwtRegisteredClaimNames.NameId, IsRequired = true)]
    public Guid UserId { get; init; }
}
