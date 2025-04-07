using System.IdentityModel.Tokens.Jwt;

using FastEndpoints;

namespace SourceName.Contracts.ToDos.Requests;

public class GetToDosFilteredRequest(
    string orderBy = "DisplayOrder",
    int? limit = 25,
    bool? isDescending = false,
    string? nextPageToken = null,
    string? title = null,
    bool? isCompleted = null)
{
    public string OrderBy { get; } = orderBy;
    public int? Limit { get; } = limit;
    public bool? IsDescending { get; } = isDescending;
    public string? NextPageToken { get; } = nextPageToken;
    public string? Title { get; } = title;
    public bool? IsCompleted { get; } = isCompleted;

    [FromClaim(JwtRegisteredClaimNames.NameId, IsRequired = true)]
    public Guid UserId { get; init; }
}
