using System.Text.Json.Serialization;

using SourceName.Domain.BaseEntity;

namespace SourceName.Domain.ToDos;

public class ToDoEntity : AuditableGuidEntity
{
    public Guid CreatedByUserId { get; }
    public string Title { get; private set; }
    public bool IsCompleted { get; private set; }
    public int? DisplayOrder { get; private set; }
    
    public ToDoEntity(
        Guid createdByUserid,
        string title,
        int displayOrder) : base()
    {
        CreatedByUserId = createdByUserid;
        Title = title;
        DisplayOrder = displayOrder;
        IsCompleted = false;
    }
    
    [JsonConstructor]
    public ToDoEntity(
        Guid id, 
        Guid createdByUserid, 
        string title,
        bool isCompleted,
        DateTime createdAt, 
        DateTime updatedAt, 
        int? displayOrder = null) 
        : base(id, createdAt, updatedAt)
    {
        CreatedByUserId = createdByUserid;
        Title = title;
        DisplayOrder = displayOrder;
        IsCompleted = isCompleted;
    }

    public void Update(string title, bool isCompleted)
    {
        Title = title;
        IsCompleted = isCompleted;

        if (isCompleted)
        {
            DisplayOrder = null;
        }
        
        Update();
    }

    public void UpdateOrder(int order)
    {
        if (IsCompleted)
        {
            return;
        }
        
        DisplayOrder = order;
        Update();
    }
}
