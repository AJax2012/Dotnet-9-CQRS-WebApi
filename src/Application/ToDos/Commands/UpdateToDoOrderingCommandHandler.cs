using System.Collections.Immutable;

using ErrorOr;

using FastEndpoints;

using Microsoft.Extensions.Logging;

using SourceName.Application.ToDos.Contracts;
using SourceName.Application.ToDos.Models;
using SourceName.Application.ToDos.Queries;
using SourceName.Domain.ToDos;

namespace SourceName.Application.ToDos.Commands;

public record UpdateToDoOrderingCommand(Dictionary<Guid, int> ToDos, Guid UserId) : ICommand<ErrorOr<Success>>;

public class UpdateToDoOrderingCommandHandler(IToDosRepository toDosRepository, ILoggerFactory logger)
    : ICommandHandler<UpdateToDoOrderingCommand, ErrorOr<Success>>
{
    private readonly IToDosRepository _toDosRepository = toDosRepository;
    private readonly ILogger _logger = logger.CreateLogger<UpdateToDoOrderingCommandHandler>();

    public async Task<ErrorOr<Success>> ExecuteAsync(UpdateToDoOrderingCommand request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        var query = new GetToDosFilteredQuery
        (
            OrderBy: ToDosOrderBy.DisplayOrder.ToStringFast(),
            IsDescending: true,
            Ids: request.ToDos.Keys.ToList()
        );

        var toDos = (await _toDosRepository.GetFilteredAsync(query, null, ct))
            .ToImmutableList();

        if (toDos.IsEmpty)
        {
            _logger.LogWarning("No ToDos found");
            return ToDoErrors.NotFound;
        }

        if (toDos.Any(x => x.CreatedByUserId != request.UserId))
        {
            _logger.LogWarning("Not all ToDos belong to user {UserId}", request.UserId);
            return ToDoErrors.NotFound;
        }

        foreach (var entity in toDos)
        {
            var order = request.ToDos[entity.Id];
            entity.UpdateOrder(order);
        }

        var rowsUpdated = await _toDosRepository.UpdateOrderAsync(toDos, ct);

        if (rowsUpdated < 1)
        {
            _logger.LogError("Failed to update order");
            return ToDoErrors.SqlError;
        }

        return Result.Success;
    }
}
