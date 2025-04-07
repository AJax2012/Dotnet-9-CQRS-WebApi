using System.Collections.Immutable;

namespace SourceName.Application.ToDos.Models;

public record ToDos(
    ImmutableList<ToDo> Items,
    bool HasNextPage,
    string NextPageToken);
