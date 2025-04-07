namespace SourceName.Contracts.ToDos.Responses;

public record ToDosResponse(IReadOnlyList<ToDoResource> Items, bool HasNextPage, string NextPageToken);
