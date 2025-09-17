using MELT;

using Microsoft.Extensions.Logging;

using SourceName.Application.ToDos.Commands;
using SourceName.Application.ToDos.Contracts;
using SourceName.Application.ToDos.Models;
using SourceName.Domain.ToDos;
using SourceName.TestUtils.ToDos;

namespace SourceName.Test.Application.ToDos.Commands;

public class UpdateToDoCommandHandlerTest
{
    private readonly IToDosRepository _toDoRepository = Substitute.For<IToDosRepository>();
    private readonly ITestLoggerFactory _loggerFactory = TestLoggerFactory.Create();
    private readonly ILogger<UpdateToDoCommandHandler> _logger;
    private readonly UpdateToDoCommandHandler _sut;

    public UpdateToDoCommandHandlerTest()
    {
        _logger = _loggerFactory.CreateLogger<UpdateToDoCommandHandler>();
        _sut = new UpdateToDoCommandHandler(_toDoRepository, _loggerFactory);
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

        var log = Assert.Single(_loggerFactory.Sink.LogEntries);
        Assert.Equal(LogLevel.Warning, log.LogLevel);
        Assert.Equal("Todo with id {Id} not found", log.OriginalFormat);
        LoggingAssert.Contains("Id", command.Id, log.Properties);

        Assert.Single(actual.Errors);
        Assert.Equal(ToDoErrors.NotFound, actual.FirstError);
    }

    [Fact]
    public async Task ExecuteAsync_LogsAndReturnsNotFound_WhenToDoExistsButUserIdDoesNotMatch()
    {
        var toDoEntity = ToDoFaker.Generate().First();
        var command = UpdateToDoCommandFaker.Faker.Generate();

        _toDoRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(toDoEntity);

        var actual = await _sut.ExecuteAsync(command, CancellationToken.None);

        var log = Assert.Single(_loggerFactory.Sink.LogEntries);
        Assert.Equal(LogLevel.Warning, log.LogLevel);
        Assert.Equal("Todo with id {Id} does not belong to user {UserId}", log.OriginalFormat);
        LoggingAssert.Contains("Id", command.Id, log.Properties);
        LoggingAssert.Contains("UserId", command.UserId, log.Properties);

        Assert.Single(actual.Errors);
        Assert.Equal(ToDoErrors.NotFound, actual.FirstError);
    }

    [Fact]
    public async Task ExecuteAsync_CallsUpdateAsync_WhenToDoExistsAndUserIdMatches()
    {
        var toDoEntity = ToDoFaker.Generate().First();

        var request = UpdateToDoCommandFaker.Faker
            .RuleFor(x => x.Id, toDoEntity.Id)
            .RuleFor(x => x.UserId, toDoEntity.CreatedByUserId)
            .Generate();

        _toDoRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(toDoEntity);

        await _sut.ExecuteAsync(request, CancellationToken.None);

        await _toDoRepository.Received()
            .UpdateAsync(Arg.Is<ToDo>(x =>
                    x.Title.Value == request.Title &&
                    x.Status.IsCompleted == request.IsCompleted),
                CancellationToken.None);
    }

    [Fact]
    public async Task ExecuteAsync_LogsAndReturnsSqlError_WhenUpdateAsyncFails()
    {
        var toDoEntity = ToDoFaker.Generate().First();

        var request = UpdateToDoCommandFaker.Faker
            .RuleFor(x => x.Id, toDoEntity.Id)
            .RuleFor(x => x.UserId, toDoEntity.CreatedByUserId)
            .Generate();

        _toDoRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(toDoEntity);

        _toDoRepository.UpdateAsync(Arg.Any<ToDo>(), Arg.Any<CancellationToken>())
            .Returns(0);

        var actual = await _sut.ExecuteAsync(request, CancellationToken.None);

        var log = Assert.Single(_loggerFactory.Sink.LogEntries);
        Assert.Equal(LogLevel.Error, log.LogLevel);
        Assert.Equal("Failed to update todo with id {Id}", log.OriginalFormat);
        LoggingAssert.Contains("Id", request.Id, log.Properties);

        Assert.Single(actual.Errors);
        Assert.Equal(ToDoErrors.SqlError, actual.FirstError);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsToDo_WhenUpdateAsyncSucceeds()
    {
        var toDoEntity = ToDoFaker.Generate().First();

        var request = UpdateToDoCommandFaker.Faker
            .RuleFor(x => x.Id, toDoEntity.Id)
            .RuleFor(x => x.UserId, toDoEntity.CreatedByUserId)
            .Generate();

        _toDoRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(toDoEntity);

        _toDoRepository.UpdateAsync(Arg.Any<ToDo>(), Arg.Any<CancellationToken>())
            .Returns(1);

        var actual = await _sut.ExecuteAsync(request, CancellationToken.None);

        Assert.Empty(actual.ErrorsOrEmptyList);

        toDoEntity.Update(request.Title, request.IsCompleted);
        var expected = toDoEntity.MapFromDomainModel();

        Assert.Equivalent(expected, actual.Value);
    }
}
