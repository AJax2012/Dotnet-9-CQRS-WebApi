using ErrorOr;
using FastEndpoints;
using Serilog;

using SourceName.Application.ToDos.Contracts;
using SourceName.Application.ToDos.Queries;
using SourceName.Contracts.ToDos;
using SourceName.Domain.ToDos;

namespace SourceName.Application.ToDos.Commands;

public record CreateToDoCommand(Guid UserId, string Title) : ICommand<ErrorOr<Guid>>;

public class CreateToDoCommandHandler(IToDosRepository toDoRepository, ILogger logger) : ICommandHandler<CreateToDoCommand, ErrorOr<Guid>>
{
    private readonly IToDosRepository _toDoRepository = toDoRepository;
    private readonly ILogger _logger = logger;

    public async Task<ErrorOr<Guid>> ExecuteAsync(CreateToDoCommand request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existingToDo = await _toDoRepository.GetByTitleAsync(request.Title, request.UserId, ct);

        if (existingToDo is not null)
        {
            _logger.Information("ToDo with toDoTitle {Title} already exists for user {UserId}", request.Title, request.UserId);
            return ToDoErrors.Conflict;
        }

        var toDoCount = await _toDoRepository.GetCountAsync(new GetToDosFilteredQuery(
                OrderBy: ToDosOrderBy.DisplayOrder.ToStringFast(),
                IsDescending: true,
                IsCompleted: false,
                Ids: [],
                Limit: null,
                NextPageToken: null,
                Title: null
            ), ct);

        var toDo = new ToDoEntity(request.UserId, new(request.Title), toDoCount + 1);

        var rowsAffected = await _toDoRepository.CreateAsync(toDo, ct);

        if (rowsAffected < 1)
        {
            _logger.Error("Failed to create ToDo for user {UserId}", request.UserId);
            return ToDoErrors.SqlError;
        }
        
        return toDo.Id;
    }
}
