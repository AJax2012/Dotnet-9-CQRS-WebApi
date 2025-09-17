using SourceName.Domain.ToDos;
#pragma warning disable CS8618, CS9264

namespace SourceName.Infrastructure.Persistence.ToDos;

public class ToDoEntity
{
    // For Dapper
    public ToDoEntity() { }

    public ToDoEntity(ToDo toDo)
    {
        Id = toDo.Id;
        CreatedByUserId = toDo.CreatedByUserId;
        Title = toDo.Title.Value;
        IsCompleted = toDo.Status.IsCompleted;
        DisplayOrder = toDo.Status.DisplayOrder;
        CreatedAt = toDo.CreatedAt;
        UpdatedAt = toDo.UpdatedAt;
    }

    internal static ToDo? ToDomainModel(ToDoEntity? dbEntity)
    {
        if (dbEntity is null)
        {
            return null;
        }

        return new(
            dbEntity.Id,
            dbEntity.CreatedByUserId,
            new(dbEntity.Title),
            new(dbEntity.IsCompleted, dbEntity.DisplayOrder),
            dbEntity.CreatedAt,
            dbEntity.UpdatedAt);
    }

    public Guid Id { get; }
    public Guid CreatedByUserId { get; }
    public string Title { get; }
    public bool IsCompleted { get; }
    public int? DisplayOrder { get; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; }
}
