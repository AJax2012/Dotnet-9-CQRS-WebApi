using ErrorOr;

using FastEndpoints;

using Serilog;

using SourceName.Application.ToDos.Contracts;
using SourceName.Application.ToDos.Models;
using SourceName.Domain.ToDos;

namespace SourceName.Application.ToDos.Commands;

public record UpdateToDoCommand(Guid Id, Guid UserId, string Title, bool IsCompleted) : ICommand<ErrorOr<ToDo>>;

public class UpdateToDoCommandHandler(IToDosRepository toDosRepository, ILogger logger)
    : ICommandHandler<UpdateToDoCommand, ErrorOr<ToDo>>
{
    private readonly IToDosRepository _toDosRepository = toDosRepository;
    private readonly ILogger _logger = logger;

    public async Task<ErrorOr<ToDo>> ExecuteAsync(UpdateToDoCommand request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);
        var todo = await _toDosRepository.GetByIdAsync(request.Id, ct);

        if (todo is null)
        {
            _logger.Warning("Todo with id {Id} not found", request.Id);
            return ToDoErrors.NotFound;
        }

        if (todo.CreatedByUserId != request.UserId)
        {
            _logger.Warning("Todo with id {Id} does not belong to user {UserId}", request.Id, request.UserId);
            return ToDoErrors.NotFound;
        }

        todo.Update(request.Title, request.IsCompleted);
        var rowsAffected = await _toDosRepository.UpdateAsync(todo, ct);

        if (rowsAffected < 1)
        {
            _logger.Error("Failed to update todo with id {Id}", request.Id);
            return ToDoErrors.SqlError;
        }

        return todo.MapFromEntity();
    }
}
