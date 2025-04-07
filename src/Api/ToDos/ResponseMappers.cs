using System.Collections.Immutable;

using SourceName.Application.ToDos.Models;

namespace SourceName.Api.ToDos;

internal static class ResponseMappers
{
    internal static ToDoResource MapToResponse(this ToDo e) => new(e.Id, e.Title, e.IsCompleted, e.Order ?? 0);
    internal static IReadOnlyList<ToDoResource> MapToResponse(this IReadOnlyList<ToDo> e) => e.Select(MapToResponse).ToImmutableList();
}
