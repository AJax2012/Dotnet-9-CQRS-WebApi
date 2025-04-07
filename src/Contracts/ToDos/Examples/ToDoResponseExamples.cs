using SourceName.Contracts.ToDos.Responses;

namespace SourceName.Contracts.ToDos.Examples;

public static class ToDoResponseExamples
{
    public static readonly ToDoResource ToDoResource =
        new(ToDoConstants.Id, ToDoConstants.Title, false, ToDoConstants.Order);

    public static readonly ToDosResponse ToDosResponse = 
        new([ToDoResource], true, ToDoConstants.NextPageToken);
    
    public static readonly CreateToDoResponse CreateToDoResponse = new(ToDoConstants.Id);

    public const string NotFoundResponse = "ToDo not found.";
    public const string SqlErrorResponse = "Could not save ToDo.";
}
