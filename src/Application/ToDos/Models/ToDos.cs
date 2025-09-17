using System.Collections.Immutable;

namespace SourceName.Application.ToDos.Models;

public record ToDos(
    ImmutableList<ToDoDto> Items,
    bool HasNextPage,
    string NextPageToken);
