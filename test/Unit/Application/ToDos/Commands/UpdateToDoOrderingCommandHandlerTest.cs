using ErrorOr;

using Serilog;

using SourceName.Application.ToDos.Commands;
using SourceName.Application.ToDos.Contracts;
using SourceName.Application.ToDos.Queries;
using SourceName.Contracts.ToDos;
using SourceName.Domain.ToDos;
using SourceName.TestUtils.ToDos;

namespace SourceName.Test.Application.ToDos.Commands;

public class UpdateToDoOrderingCommandHandlerTest
{
    private readonly IToDosRepository _toDoRepository = Substitute.For<IToDosRepository>();
    private readonly ILogger _logger = Substitute.For<ILogger>();
    private readonly UpdateToDoOrderingCommandHandler _sut;

    public UpdateToDoOrderingCommandHandlerTest()
    {
        _sut = new(_toDoRepository, _logger);
    }

    [Fact]
    public async Task ExecuteAsync_ThrowsArgumentNullException_WhenCommandIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.ExecuteAsync(null!, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_CallsGetFilteredAsync_WhenRequestIsNotNull()
    {
        await _sut.ExecuteAsync(new([], Guid.NewGuid()), CancellationToken.None);

        await _toDoRepository.Received()
            .GetFilteredAsync(Arg.Is<GetToDosFilteredQuery>(x =>
                    x.OrderBy == ToDosOrderBy.DisplayOrder.ToStringFast() &&
                    x.IsDescending == true &&
                    x.Ids.Count == 0
                ),
                null,
                CancellationToken.None);
    }

    [Fact]
    public async Task ExecuteAsync_LogsAndReturnsNotFound_WhenToDoDoesNotExist()
    {
        var actual = await _sut.ExecuteAsync(new([], Guid.NewGuid()), CancellationToken.None);

        _logger.Received()
            .Warning("No ToDos found");

        Assert.Single(actual.Errors);
        Assert.Equal(ToDoErrors.NotFound, actual.FirstError);
    }

    [Fact]
    public async Task ExecuteAsync_LogsAndReturnsNotFound_WhenToDoExistsButUserIdDoesNotMatch()
    {
        var requestUserId = Guid.NewGuid();
        var toDos = ToDoEntityFaker.Generate(5);

        _toDoRepository.GetFilteredAsync(Arg.Any<GetToDosFilteredQuery>(), null, Arg.Any<CancellationToken>())
            .Returns(toDos);

        var actual = await _sut.ExecuteAsync(new([], requestUserId), CancellationToken.None);

        _logger.Received()
            .Warning("Not all ToDos belong to user {UserId}", requestUserId);

        Assert.Single(actual.Errors);
        Assert.Equal(ToDoErrors.NotFound, actual.FirstError);
    }

    [Fact]
    public async Task ExecuteAsync_CallsUpdateAsync_WhenToDoExistsAndUserIdMatches()
    {
        var toDos = ToDoEntityFaker.Generate(5);
        var requestDictionary = new Dictionary<Guid, int>();

        toDos.Reverse();
        foreach (var toDo in toDos)
        {
            var order = toDos.IndexOf(toDo) + 1;
            requestDictionary.Add(toDo.Id, order);
        }

        _toDoRepository.GetFilteredAsync(Arg.Any<GetToDosFilteredQuery>(), null, Arg.Any<CancellationToken>())
            .Returns(toDos);

        await _sut.ExecuteAsync(new(requestDictionary, ToDoEntityFaker.EntityCreatedByUserId), CancellationToken.None);

        foreach (var toDo in toDos)
        {
            toDo.UpdateOrder(requestDictionary[toDo.Id]);
            await _toDoRepository.Received()
                .UpdateOrderAsync(Arg.Is<IReadOnlyList<ToDoEntity>>(x =>
                        x.Count == toDos.Count &&
                        x.Contains(toDo)
                    ), CancellationToken.None);
        }
    }

    [Fact]
    public async Task ExecuteAsync_LogsAndReturnsSqlError_WhenUpdateAsyncFails()
    {
        var toDos = ToDoEntityFaker.Generate(5);

        var requestDictionary = toDos.ToDictionary(x => x.Id, y => y.Status.DisplayOrder!.Value);

        _toDoRepository.GetFilteredAsync(Arg.Any<GetToDosFilteredQuery>(), null, Arg.Any<CancellationToken>())
            .Returns(toDos);

        _toDoRepository.UpdateOrderAsync(Arg.Any<IReadOnlyList<ToDoEntity>>(), Arg.Any<CancellationToken>())
            .Returns(0);

        var actual = await _sut.ExecuteAsync(new(requestDictionary, ToDoEntityFaker.EntityCreatedByUserId), CancellationToken.None);

        _logger.Received()
            .Error("Failed to update order");

        Assert.Single(actual.Errors);
        Assert.Equal(ToDoErrors.SqlError, actual.FirstError);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsSuccess_WhenUpdateAsyncSucceeds()
    {
        var toDos = ToDoEntityFaker.Generate(5);

        var requestDictionary = toDos.ToDictionary(x => x.Id, y => y.Status.DisplayOrder!.Value);

        _toDoRepository.GetFilteredAsync(Arg.Any<GetToDosFilteredQuery>(), null, Arg.Any<CancellationToken>())
            .Returns(toDos);

        _toDoRepository.UpdateOrderAsync(Arg.Any<IReadOnlyList<ToDoEntity>>(), Arg.Any<CancellationToken>())
            .Returns(5);

        var actual = await _sut.ExecuteAsync(new(requestDictionary, ToDoEntityFaker.EntityCreatedByUserId), CancellationToken.None);

        Assert.Empty(actual.ErrorsOrEmptyList);
        Assert.Equal(Result.Success, actual.Value);
    }
}
