namespace SourceName.Contracts.ToDos.Responses;

public record ToDoResource(
    Guid Id,
    string Title,
    bool IsCompleted,
    int DisplayOrder);
