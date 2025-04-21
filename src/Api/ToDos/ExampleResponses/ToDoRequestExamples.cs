using SourceName.Api.ToDos.Create;
using SourceName.Api.ToDos.Delete;
using SourceName.Api.ToDos.GetByFilter;
using SourceName.Api.ToDos.GetById;
using SourceName.Api.ToDos.Update;
using SourceName.Api.ToDos.UpdateOrder;
using SourceName.Contracts.ToDos;

namespace SourceName.Api.ToDos.ExampleResponses;

internal static class ToDoRequestExamples
{
    private const string Title = "Example ToDo";
    private const string NextPageToken = "eyJJZCI6IjIwMjJmODdhLWE1YjItNGQxZi1iOGUyLTRhNzY1MjU3YzMzMiIsIlRpdGxlIjoiRXhhbXBsZSBUb0RvIn0=";
    private static readonly Guid ExampleId = Guid.NewGuid();
    
    internal static readonly CreateToDoRequest CreateToDoRequest = new() { Title = Title };
    internal static readonly UpdateToDoRequest UpdateToDoRequest = new() { Id = ExampleId, Title = Title, IsCompleted = false };
    internal static readonly UpdateToDoOrderingRequest UpdateToDoOrderingRequest = new() { ToDos = new(){{ ExampleId, 1 }, { Guid.NewGuid(), 2 }}};
    internal static readonly DeleteToDoRequest DeleteToDoRequest = new() { Id = ExampleId };
    internal static readonly GetToDoByIdRequest GetToDoByIdRequest = new() { Id = ExampleId };

    internal static readonly GetToDosFilteredRequest GetToDosFilteredRequestWithAllProperties = new()
    {
        OrderBy = ToDosOrderBy.DisplayOrder.ToStringFast(),
        Limit = 25,
        IsDescending = true,
        NextPageToken = NextPageToken,
        Title = Title,
        IsCompleted = false
    };

    internal static readonly GetToDosFilteredRequest GetToDosFilteredRequestWithoutNullableProperties = new()
    {
        OrderBy = ToDosOrderBy.DisplayOrder.ToStringFast(),
    };
}
