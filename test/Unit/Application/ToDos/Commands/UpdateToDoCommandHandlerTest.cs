using Serilog;

using SourceName.Application.ToDos.Commands;
using SourceName.Application.ToDos.Contracts;
using SourceName.Application.ToDos.Models;
using SourceName.Domain.ToDos;
using SourceName.TestUtils.ToDos;

namespace SourceName.Test.Application.ToDos.Commands;

public class UpdateToDoCommandHandlerTest
{
    private readonly IToDosRepository _toDoRepository = Substitute.For<IToDosRepository>();
    private readonly ILogger _logger = Substitute.For<ILogger>();
    private readonly UpdateToDoCommandHandler _sut;

    public UpdateToDoCommandHandlerTest()
    {
        _sut = new UpdateToDoCommandHandler(_toDoRepository, _logger);
    }

    [Fact]
    public async Task ExecuteAsync_ThrowsArgumentNullException_WhenCommandIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.ExecuteAsync(null!, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_CallsGetByIdAsync_WhenRequestIsNotNull()
    {
        var command = UpdateToDoCommandFaker.Faker.Generate();
        await _sut.ExecuteAsync(command, CancellationToken.None);

        await _toDoRepository.Received()
            .GetByIdAsync(command.Id, CancellationToken.None);
    }

    [Fact]
    public async Task ExecuteAsync_LogsAndReturnsNotFound_WhenToDoDoesNotExist()
    {
        var command = UpdateToDoCommandFaker.Faker.Generate();
        var actual = await _sut.ExecuteAsync(command, CancellationToken.None);

        _logger.Received()
            .Warning("Todo with id {Id} not found", command.Id);

        Assert.Single(actual.Errors);
        Assert.Equal(ToDoErrors.NotFound, actual.FirstError);
    }

    [Fact]
    public async Task ExecuteAsync_LogsAndReturnsNotFound_WhenToDoExistsButUserIdDoesNotMatch()
    {
        var toDoEntity = ToDoEntityFaker.Generate().First();
        var command = UpdateToDoCommandFaker.Faker.Generate();

        _toDoRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(toDoEntity);

        var actual = await _sut.ExecuteAsync(command, CancellationToken.None);

        _logger.Received()
            .Warning(
                "Todo with id {Id} does not belong to user {UserId}",
                command.Id,
                command.UserId);

        Assert.Single(actual.Errors);
        Assert.Equal(ToDoErrors.NotFound, actual.FirstError);
    }

    [Fact]
    public async Task ExecuteAsync_CallsUpdateAsync_WhenToDoExistsAndUserIdMatches()
    {
        var toDoEntity = ToDoEntityFaker.Generate().First();

        var request = UpdateToDoCommandFaker.Faker
            .RuleFor(x => x.Id, toDoEntity.Id)
            .RuleFor(x => x.UserId, toDoEntity.CreatedByUserId)
            .Generate();

        _toDoRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(toDoEntity);

        await _sut.ExecuteAsync(request, CancellationToken.None);

        await _toDoRepository.Received()
            .UpdateAsync(Arg.Is<ToDoEntity>(x =>
                    x.Title.Value == request.Title &&
                    x.Status.IsCompleted == request.IsCompleted),
                CancellationToken.None);
    }

    [Fact]
    public async Task ExecuteAsync_LogsAndReturnsSqlError_WhenUpdateAsyncFails()
    {
        var toDoEntity = ToDoEntityFaker.Generate().First();

        var request = UpdateToDoCommandFaker.Faker
            .RuleFor(x => x.Id, toDoEntity.Id)
            .RuleFor(x => x.UserId, toDoEntity.CreatedByUserId)
            .Generate();

        _toDoRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(toDoEntity);

        _toDoRepository.UpdateAsync(Arg.Any<ToDoEntity>(), Arg.Any<CancellationToken>())
            .Returns(0);

        var actual = await _sut.ExecuteAsync(request, CancellationToken.None);

        _logger.Received()
            .Error("Failed to update todo with id {Id}", request.Id);

        Assert.Single(actual.Errors);
        Assert.Equal(ToDoErrors.SqlError, actual.FirstError);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsToDo_WhenUpdateAsyncSucceeds()
    {
        var toDoEntity = ToDoEntityFaker.Generate().First();

        var request = UpdateToDoCommandFaker.Faker
            .RuleFor(x => x.Id, toDoEntity.Id)
            .RuleFor(x => x.UserId, toDoEntity.CreatedByUserId)
            .Generate();

        _toDoRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(toDoEntity);

        _toDoRepository.UpdateAsync(Arg.Any<ToDoEntity>(), Arg.Any<CancellationToken>())
            .Returns(1);

        var actual = await _sut.ExecuteAsync(request, CancellationToken.None);

        Assert.Empty(actual.ErrorsOrEmptyList);

        toDoEntity.Update(request.Title, request.IsCompleted);
        var expected = toDoEntity.MapFromEntity();

        Assert.Equivalent(expected, actual.Value);
    }
}
