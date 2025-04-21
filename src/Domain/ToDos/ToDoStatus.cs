namespace SourceName.Domain.ToDos;

public class ToDoStatus
{
    public bool IsCompleted { get; private set; }
    public int? DisplayOrder { get; private set; }

    public ToDoStatus(bool isCompleted, int? displayOrder)
    {
        IsCompleted = isCompleted;
        SetDisplayOrder(displayOrder);
    }
    
    internal void Update(bool isCompleted, int? displayOrder)
    {
        IsCompleted = isCompleted;
        SetDisplayOrder(displayOrder);
    }

    internal void SetDisplayOrder(int? displayOrder)
    {
        if (IsCompleted)
        {
            DisplayOrder = null;
            return;
        }
        
        DisplayOrder = displayOrder ?? DisplayOrder;
    }
}
