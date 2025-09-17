using System.Collections.Immutable;

using SourceName.Application.ToDos.Models;

namespace SourceName.Api.ToDos;

internal static class ResponseMappers
{
    internal static ToDoResource MapToResponse(this ToDoDto e) => new(e.Id, e.Title, e.IsCompleted, e.Order ?? 0);
    internal static IReadOnlyList<ToDoResource> MapToResponse(this IReadOnlyList<ToDoDto> e) => e.Select(MapToResponse).ToImmutableList();
}
