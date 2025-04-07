using System.IdentityModel.Tokens.Jwt;

using FastEndpoints;

namespace SourceName.Api.ToDos.GetByFilter;

/// <summary>
/// Request to get To Dos with filter 
/// </summary>
public class GetToDosFilteredRequest(
    string orderBy = "DisplayOrder",
    int? limit = 25,
    bool? isDescending = false,
    string? nextPageToken = null,
    string? title = null,
    bool? isCompleted = null)
{
    /// <summary>
    /// Property on which to order list
    /// </summary>
    public string OrderBy { get; } = orderBy;
    
    /// <summary>
    /// Number of To Dos to return 
    /// </summary>
    public int? Limit { get; } = limit;
    
    /// <summary>
    /// Direction of OrderBy 
    /// </summary>
    public bool? IsDescending { get; } = isDescending;
    
    /// <summary>
    /// Next item to start at
    /// </summary>
    public string? NextPageToken { get; } = nextPageToken;
    
    /// <summary>
    /// Title of to do
    /// </summary>
    public string? Title { get; } = title;
    
    /// <summary>
    /// Is to do completed?
    /// </summary>
    public bool? IsCompleted { get; } = isCompleted;

    /// <summary>
    /// User Id from auth token.
    /// </summary>
    [FromClaim(JwtRegisteredClaimNames.NameId, IsRequired = true)]
    public Guid UserId { get; init; }
}
