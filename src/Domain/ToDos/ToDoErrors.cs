using System.Diagnostics.CodeAnalysis;

using ErrorOr;

namespace SourceName.Domain.ToDos;

[SuppressMessage("Maintainability", "CA1515:Consider making public types internal")]
public static class ToDoErrors
{
    public static Error NotFound => Error.NotFound(code: "ToDo.NotFound", description: "ToDo not found.");
    public static Error Conflict => Error.Conflict(code: "ToDo.Conflict", description: "ToDo with Title already exists.");
    public static Error SqlError => Error.Unexpected(code: "ToDo.SqlError", description: "Could not save ToDo.");
}
