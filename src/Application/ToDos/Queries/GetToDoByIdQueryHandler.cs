using ErrorOr;
using FastEndpoints;
using Serilog;

using SourceName.Application.ToDos.Contracts;
using SourceName.Application.ToDos.Models;
using SourceName.Domain.ToDos;

namespace SourceName.Application.ToDos.Queries;

public record GetToDoByIdQuery(Guid Id, Guid UserId) : ICommand<ErrorOr<ToDo>>;

public class GetToDoByIdQueryHandler(IToDosRepository toDosRepository, ILogger logger) : ICommandHandler<GetToDoByIdQuery, ErrorOr<ToDo>>
{
    private readonly IToDosRepository _toDosRepository = toDosRepository;
    private readonly ILogger _logger = logger;

    public async Task<ErrorOr<ToDo>> ExecuteAsync(GetToDoByIdQuery request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);
        var toDo = await _toDosRepository.GetByIdAsync(request.Id, ct);

        if (toDo is null)
        {
            _logger.Warning("Todo with id {Id} not found", request.Id);
            return ToDoErrors.NotFound;
        }
        
        if (toDo.CreatedByUserId != request.UserId)
        {
            _logger.Warning("Todo with id {Id} does not belong to user {UserId}", request.Id, request.UserId);
            return ToDoErrors.NotFound;
        }
        
        return toDo.MapFromEntity();
    }
}
