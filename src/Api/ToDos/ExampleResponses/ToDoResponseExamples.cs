using SourceName.Api.ToDos.Create;
using SourceName.Api.ToDos.GetByFilter;

namespace SourceName.Api.ToDos.ExampleResponses;

internal static class ToDoResponseExamples
{
    private const int Order = 1;
    private const string Title = "Example ToDo";
    private const string NextPageToken = "eyJJZCI6IjIwMjJmODdhLWE1YjItNGQxZi1iOGUyLTRhNzY1MjU3YzMzMiIsIlRpdGxlIjoiRXhhbXBsZSBUb0RvIn0=";
    private static readonly Guid ExampleId = Guid.NewGuid();

    internal static readonly ToDoResource ToDoResource =
        new(ExampleId, Title, false, Order);

    internal static readonly ToDosResponse ToDosResponse =
        new([ToDoResource], true, NextPageToken);

    internal static readonly CreateToDoResponse CreateToDoResponse = new(ExampleId);

    internal const string NotFoundResponse = "ToDo not found.";
    internal const string SqlErrorResponse = "Could not save ToDo.";
}
