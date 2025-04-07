using System.Collections.Immutable;

using ErrorOr;
using FastEndpoints;
using Serilog;

using SourceName.Application.ToDos.Contracts;
using SourceName.Application.ToDos.Models;
using SourceName.Domain.ToDos;

namespace SourceName.Application.ToDos.Queries;

public record GetToDosFilteredQuery(
    string OrderBy,
    bool IsDescending,
    List<Guid> Ids,
    int? Limit = null,
    string? NextPageToken = null,
    string? Title = null,
    bool? IsCompleted = null) : ICommand<ErrorOr<Models.ToDos>>;

public class GetFilteredToDosQueryHandler(IToDosRepository toDoRepository, ILogger logger) : 
    ICommandHandler<GetToDosFilteredQuery, ErrorOr<Models.ToDos>>
{
    private readonly IToDosRepository _toDoRepository = toDoRepository;
    private readonly ILogger _logger = logger;

    public async Task<ErrorOr<Models.ToDos>> ExecuteAsync(GetToDosFilteredQuery request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        var cursor = ToDoNextResultToken.DecodeToken(request.NextPageToken);
        var filteredToDos = (await _toDoRepository.GetFilteredAsync(request, cursor, ct))
            .ToImmutableList();

        if (filteredToDos.IsEmpty)
        {
            _logger.Debug("No ToDos found");
            return ToDoErrors.NotFound;
        }

        var mappedToDos = filteredToDos
            .Take(request.Limit ?? filteredToDos.Count)
            .Select(x => x.MapFromEntity())
            .ToImmutableList();
        
        var hasNextPage = filteredToDos.Count > (request.Limit ?? filteredToDos.Count);

        return new Models.ToDos(
            mappedToDos,
            hasNextPage,
            ToDoNextResultToken.EncodeToken(filteredToDos.Last()));
    }
}
