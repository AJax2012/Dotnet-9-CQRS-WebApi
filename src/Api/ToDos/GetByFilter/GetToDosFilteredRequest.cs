using System.IdentityModel.Tokens.Jwt;
using FastEndpoints;
using SourceName.Contracts.ToDos;

namespace SourceName.Api.ToDos.GetByFilter;

/// <summary>
/// Request to get To Dos with filter 
/// </summary>
public class GetToDosFilteredRequest
{
    /// <summary>
    /// Property on which to order list
    /// </summary>
    public string OrderBy { get; init; } = ToDosOrderBy.DisplayOrder.ToStringFast();
    
    /// <summary>
    /// Number of To Dos to return 
    /// </summary>
    public int? Limit { get; init; } = 25;
    
    /// <summary>
    /// Direction of OrderBy 
    /// </summary>
    public bool? IsDescending { get; init; } = false;
    
    /// <summary>
    /// Next item to start at
    /// </summary>
    public string? NextPageToken { get; init; }
    
    /// <summary>
    /// Title of to do
    /// </summary>
    public string? Title { get; init; }
    
    /// <summary>
    /// Is to do completed?
    /// </summary>
    public bool? IsCompleted { get; init; } = false;

    /// <summary>
    /// User Id from auth token.
    /// </summary>
    [FromClaim(JwtRegisteredClaimNames.NameId, IsRequired = true)]
    public Guid UserId { get; init; }
}
