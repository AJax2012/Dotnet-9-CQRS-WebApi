using System.Text;
using System.Text.Json;

using SourceName.Domain.ToDos;

namespace SourceName.Application.ToDos.Models;

public static class ToDoNextResultToken
{
    public static ToDoEntity? DecodeToken(string? token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return null;
        }
        
        var bytes = Convert.FromBase64String(token);
        var json = Encoding.UTF8.GetString(bytes);
        return JsonSerializer.Deserialize<ToDoEntity>(json);
    }

    public static string EncodeToken(ToDoEntity? entity)
    {
        if (entity is null)
        {
            return string.Empty;
        }
        
        var json = JsonSerializer.Serialize(entity);
        var bytes = Encoding.UTF8.GetBytes(json);
        return Convert.ToBase64String(bytes);
    }
}
