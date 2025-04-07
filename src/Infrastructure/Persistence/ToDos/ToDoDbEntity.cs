using SourceName.Domain.ToDos;

namespace SourceName.Infrastructure.Persistence.ToDos;

public class ToDoDbEntity
{
    public ToDoDbEntity() { }
    
    public ToDoDbEntity(ToDoEntity toDo)
    {
        Id = toDo.Id;
        CreatedByUserId = toDo.CreatedByUserId;
        Title = toDo.Title;
        IsCompleted = toDo.IsCompleted;
        DisplayOrder = toDo.DisplayOrder;
        CreatedAt = toDo.CreatedAt;
        UpdatedAt = toDo.UpdatedAt;
    }
    
    internal static ToDoEntity? ToEntity(ToDoDbEntity? dbEntity)
    {
        if (dbEntity is null)
        {
            return null;
        }

        return new(
            dbEntity.Id,
            dbEntity.CreatedByUserId,
            dbEntity.Title,
            dbEntity.IsCompleted,
            dbEntity.CreatedAt,
            dbEntity.UpdatedAt,
            dbEntity.DisplayOrder);
    }
    
    public Guid Id { get; }
    public Guid CreatedByUserId { get; }
    public string Title { get; }
    public bool IsCompleted { get; }
    public int? DisplayOrder { get; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; }
}
