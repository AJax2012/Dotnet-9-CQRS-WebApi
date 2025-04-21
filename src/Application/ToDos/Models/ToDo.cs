using SourceName.Domain.ToDos;

namespace SourceName.Application.ToDos.Models;

public record ToDo(
    Guid Id,
    Guid CreatedByUserId,
    string Title,
    bool IsCompleted,
    int? Order = null);

public static class ToDoExtensions
{
    public static ToDo MapFromEntity(this ToDoEntity toDo) =>
        new(toDo.Id, toDo.CreatedByUserId, toDo.Title.Value, toDo.Status.IsCompleted, toDo.Status.DisplayOrder);
}
