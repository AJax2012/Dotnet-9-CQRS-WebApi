using SourceName.Contracts.ToDos.Requests;

namespace SourceName.Contracts.ToDos.Examples;

public static class ToDoRequestExamples
{
    public static readonly CreateToDoRequest CreateToDoRequest = new() { Title = ToDoConstants.Title };
    public static readonly UpdateToDoRequest UpdateToDoRequest = new() { Id = ToDoConstants.Id, Title = ToDoConstants.Title, IsCompleted = false };
    public static readonly UpdateToDoOrderingRequest UpdateToDoOrderingRequest = new() { ToDos = new(){{ ToDoConstants.Id, 1 }, { Guid.NewGuid(), 2 }}};
    public static readonly DeleteToDoRequest DeleteToDoRequest = new() { Id = ToDoConstants.Id };
    public static readonly GetToDoByIdRequest GetToDoByIdRequest = new() { Id = ToDoConstants.Id };

    public static readonly GetToDosFilteredRequest GetToDosFilteredRequestWithAllProperties = new(
        ToDosOrderBy.DisplayOrder.ToStringFast(),
        25,
        true,
        ToDoConstants.NextPageToken,
        ToDoConstants.Title,
        false);

    public static readonly GetToDosFilteredRequest GetToDosFilteredRequestWithoutNullableProperties =
        new(ToDosOrderBy.DisplayOrder.ToStringFast());
}
