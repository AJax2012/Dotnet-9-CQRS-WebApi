namespace SourceName.Api.ToDos.GetByFilter;

/// <summary>
/// Response with a list of To Dos
/// </summary>
/// <param name="Items">List of To Do items</param>
/// <param name="HasNextPage">Marker if there are more items</param>
/// <param name="NextPageToken">Token with the next item to retrieve</param>
public record ToDosResponse(IReadOnlyList<ToDoResource> Items, bool HasNextPage, string NextPageToken);
