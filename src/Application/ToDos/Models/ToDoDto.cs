using SourceName.Domain.ToDos;

namespace SourceName.Application.ToDos.Models;

public record ToDoDto(
    Guid Id,
    Guid CreatedByUserId,
    string Title,
    bool IsCompleted,
    int? Order = null);

public static class ToDoDtoExtensions
{
    public static ToDoDto MapFromDomainModel(this Domain.ToDos.ToDo toDo) =>
        new(toDo.Id, toDo.CreatedByUserId, toDo.Title.Value, toDo.Status.IsCompleted, toDo.Status.DisplayOrder);
}
