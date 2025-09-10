using ErrorOr;

using FastEndpoints;

using Microsoft.Extensions.Logging;

using SourceName.Application.ToDos.Contracts;
using SourceName.Domain.ToDos;

namespace SourceName.Application.ToDos.Commands;

public record DeleteToDoCommand(Guid Id, Guid UserId) : ICommand<ErrorOr<Success>>;

public class DeleteToDoCommandHandler(IToDosRepository toDosRepository, ILoggerFactory logger)
    : ICommandHandler<DeleteToDoCommand, ErrorOr<Success>>
{
    private readonly IToDosRepository _toDosRepository = toDosRepository;
    private readonly ILogger _logger = logger.CreateLogger<DeleteToDoCommandHandler>();

    public async Task<ErrorOr<Success>> ExecuteAsync(DeleteToDoCommand request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);
        var todo = await _toDosRepository.GetByIdAsync(request.Id, ct);

        if (todo is null)
        {
            _logger.LogWarning("Todo with id {Id} not found", request.Id);
            return ToDoErrors.NotFound;
        }

        if (todo.CreatedByUserId != request.UserId)
        {
            _logger.LogWarning("Todo with id {Id} does not belong to user {UserId}", request.Id, request.UserId);
            return ToDoErrors.NotFound;
        }

        var rowsAffected = await _toDosRepository.DeleteAsync(request.Id, ct);

        if (rowsAffected < 1)
        {
            _logger.LogError("Failed to delete todo with id {Id}", request.Id);
            return ToDoErrors.SqlError;
        }

        return Result.Success;
    }
}
