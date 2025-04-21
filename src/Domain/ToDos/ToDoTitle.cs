namespace SourceName.Domain.ToDos;

public class ToDoTitle
{
    public string Value { get; private set; }

    public ToDoTitle(string value)
    {
        value = value.Trim();
        
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Title cannot be null or whitespace", nameof(value));
        }
        
        if (value.Length > 100)
        {
            throw new ArgumentException("Title cannot be longer than 100 characters", nameof(value));
        }
        
        Value = value;
    }
}
