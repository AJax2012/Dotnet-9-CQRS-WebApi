namespace SourceName.Api.ToDos;

/// <summary>
/// Shared To Do resource for responses
/// </summary>
/// <param name="Id">Id of the To Do</param>
/// <param name="Title">Title of the To Do</param>
/// <param name="IsCompleted">Is the To Do completed</param>
/// <param name="DisplayOrder">Order to display the To Do</param>
public record ToDoResource(
    Guid Id,
    string Title,
    bool IsCompleted,
    int DisplayOrder);
