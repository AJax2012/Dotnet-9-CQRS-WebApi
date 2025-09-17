using System.Text.Json.Serialization;

using SourceName.Domain.BaseEntity;

namespace SourceName.Domain.ToDos;

public class ToDo : AuditableGuidEntity
{
    public Guid CreatedByUserId { get; }
    public ToDoTitle Title { get; private set; }
    public ToDoStatus Status { get; private set; }

    public ToDo(
        Guid createdByUserid,
        ToDoTitle title,
        int displayOrder) : base()
    {
        CreatedByUserId = createdByUserid;
        Title = title;
        Status = new(false, displayOrder);
    }

    [JsonConstructor]
    public ToDo(
        Guid id,
        Guid createdByUserid,
        ToDoTitle title,
        ToDoStatus status,
        DateTime createdAt,
        DateTime updatedAt)
        : base(id, createdAt, updatedAt)
    {
        CreatedByUserId = createdByUserid;
        Title = title;
        Status = new(status.IsCompleted, status.DisplayOrder);
    }

    public void Update(string title, bool isCompleted)
    {
        Title = new(title);
        Status.Update(isCompleted, null);
        Update();
    }

    public void UpdateOrder(int order)
    {
        Status.SetDisplayOrder(order);
        Update();
    }
}
