using System.Text;
using System.Text.Json;

using SourceName.Domain.ToDos;

namespace SourceName.Application.ToDos.Models;

public static class ToDoNextResultToken
{
    public static ToDo? DecodeToken(string? token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return null;
        }

        var bytes = Convert.FromBase64String(token);
        var json = Encoding.UTF8.GetString(bytes);
        return JsonSerializer.Deserialize<ToDo>(json);
    }

    public static string EncodeToken(ToDo? entity)
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
