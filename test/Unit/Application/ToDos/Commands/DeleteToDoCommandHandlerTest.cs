using MELT;
using MELT.Xunit;
using Microsoft.Extensions.Logging;
using SourceName.Application.ToDos.Commands;
using SourceName.Application.ToDos.Contracts;
using SourceName.Domain.ToDos;
using SourceName.TestUtils.ToDos;

namespace SourceName.Test.Application.ToDos.Commands;

public class DeleteToDoCommandHandlerTest
{
    private readonly IToDosRepository _toDoRepository = Substitute.For<IToDosRepository>();
    private readonly ITestLoggerFactory _loggerFactory = TestLoggerFactory.Create();
    private readonly ILogger<DeleteToDoCommandHandler> _logger;
    private readonly DeleteToDoCommandHandler _sut;

    public DeleteToDoCommandHandlerTest()
    {
        _logger = _loggerFactory.CreateLogger<DeleteToDoCommandHandler>();
        _sut = new(_toDoRepository, _loggerFactory);
    }

    [Fact]
    public async Task ExecuteAsync_ThrowsArgumentNullException_WhenCommandIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.ExecuteAsync(null!, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_CallsGetByIdAsync_WhenRequestIsNotNull()
    {
        var command = DeleteToDoCommandFaker.Faker.Generate();
        await _sut.ExecuteAsync(command, CancellationToken.None);

        await _toDoRepository.Received()
            .GetByIdAsync(command.Id, CancellationToken.None);
    }

    [Fact]
    public async Task ExecuteAsync_LogsAndReturnsNotFound_WhenToDoDoesNotExist()
    {
        var command = DeleteToDoCommandFaker.Faker.Generate();
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
        var entity = ToDoEntityFaker.Generate().First();
        var command = DeleteToDoCommandFaker.Faker.Generate();

        _toDoRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(entity);

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
    public async Task ExecuteAsync_CallsDeleteAsync_WhenToDoExistsAndUserIdMatches()
    {
        var entity = ToDoEntityFaker.Generate().First();

        var command = DeleteToDoCommandFaker.Faker
            .RuleFor(x => x.Id, entity.Id)
            .RuleFor(x => x.UserId, entity.CreatedByUserId)
            .Generate();

        _toDoRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(entity);

        await _sut.ExecuteAsync(command, CancellationToken.None);

        await _toDoRepository.Received()
            .DeleteAsync(command.Id, CancellationToken.None);
    }

    [Fact]
    public async Task ExecuteAsync_LogsAndReturnsSqlError_WhenDeleteAsyncFails()
    {
        var entity = ToDoEntityFaker.Generate().First();

        var command = DeleteToDoCommandFaker.Faker
            .RuleFor(x => x.Id, entity.Id)
            .RuleFor(x => x.UserId, entity.CreatedByUserId)
            .Generate();

        _toDoRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(entity);

        _toDoRepository.DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(0);

        var actual = await _sut.ExecuteAsync(command, CancellationToken.None);

        var log = Assert.Single(_loggerFactory.Sink.LogEntries);
        Assert.Equal(LogLevel.Error, log.LogLevel);
        Assert.Equal("Failed to delete todo with id {Id}", log.OriginalFormat);
        LoggingAssert.Contains("Id", command.Id, log.Properties);

        Assert.Single(actual.Errors);
        Assert.Equal(ToDoErrors.SqlError, actual.FirstError);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsSuccess_WhenDeleteAsyncSucceeds()
    {
        var entity = ToDoEntityFaker.Generate().First();

        var command = DeleteToDoCommandFaker.Faker
            .RuleFor(x => x.Id, entity.Id)
            .RuleFor(x => x.UserId, entity.CreatedByUserId)
            .Generate();

        _toDoRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(entity);

        _toDoRepository.DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(1);

        var actual = await _sut.ExecuteAsync(command, CancellationToken.None);

        Assert.Empty(actual.ErrorsOrEmptyList);
    }
}
