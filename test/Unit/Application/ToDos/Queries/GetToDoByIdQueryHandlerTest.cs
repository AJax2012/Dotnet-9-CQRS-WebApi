using MELT;

using Microsoft.Extensions.Logging;

using SourceName.Application.ToDos.Contracts;
using SourceName.Application.ToDos.Models;
using SourceName.Application.ToDos.Queries;
using SourceName.Domain.ToDos;
using SourceName.TestUtils.ToDos;

namespace SourceName.Test.Application.ToDos.Queries;

public class GetToDoByIdQueryHandlerTest
{
    private readonly IToDosRepository _toDoRepository = Substitute.For<IToDosRepository>();
    private readonly ITestLoggerFactory _loggerFactory = TestLoggerFactory.Create();
    private readonly ILogger<GetToDoByIdQueryHandler> _logger;
    private readonly GetToDoByIdQueryHandler _sut;

    public GetToDoByIdQueryHandlerTest()
    {
        _logger = _loggerFactory.CreateLogger<GetToDoByIdQueryHandler>();
        _sut = new GetToDoByIdQueryHandler(_toDoRepository, _loggerFactory);
    }

    [Fact]
    public async Task ExecuteAsync_ThrowsArgumentNullException_WhenRequestIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.ExecuteAsync(null!, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_CallsGetByIdAsync_WhenRequestIsNotNull()
    {
        var request = GetToDoByIdQueryFaker.Faker.Generate();
        await _sut.ExecuteAsync(request, CancellationToken.None);

        await _toDoRepository.Received()
            .GetByIdAsync(request.Id, CancellationToken.None);
    }

    [Fact]
    public async Task ExecuteAsync_LogsAndReturnsNotFound_WhenToDoNotFound()
    {
        var request = GetToDoByIdQueryFaker.Faker.Generate();
        var actual = await _sut.ExecuteAsync(request, CancellationToken.None);

        var log = Assert.Single(_loggerFactory.Sink.LogEntries);
        Assert.Equal(LogLevel.Warning, log.LogLevel);
        Assert.Equal("Todo with id {Id} not found", log.OriginalFormat);
        LoggingAssert.Contains("Id", request.Id, log.Properties);

        Assert.Single(actual.Errors);
        Assert.Equal(ToDoErrors.NotFound, actual.FirstError);
    }

    [Fact]
    public async Task ExecuteAsync_LogsAndReturnsNotFound_WhenToDoFoundButUserIdDoesNotMatch()
    {
        var toDoEntity = ToDoFaker.Generate().First();
        var request = GetToDoByIdQueryFaker.Faker.Generate();

        _toDoRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(toDoEntity);

        var actual = await _sut.ExecuteAsync(request, CancellationToken.None);

        var log = Assert.Single(_loggerFactory.Sink.LogEntries);
        Assert.Equal(LogLevel.Warning, log.LogLevel);
        Assert.Equal("Todo with id {Id} does not belong to user {UserId}", log.OriginalFormat);
        LoggingAssert.Contains("Id", request.Id, log.Properties);
        LoggingAssert.Contains("UserId", request.UserId, log.Properties);

        Assert.Single(actual.Errors);
        Assert.Equal(ToDoErrors.NotFound, actual.FirstError);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsToDo_WhenToDoFoundAndUserIdMatches()
    {
        var toDoEntity = ToDoFaker.Generate().First();

        var request = GetToDoByIdQueryFaker.Faker
            .RuleFor(x => x.UserId, toDoEntity.CreatedByUserId)
            .RuleFor(x => x.Id, toDoEntity.Id)
            .Generate();

        _toDoRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(toDoEntity);

        var actual = await _sut.ExecuteAsync(request, CancellationToken.None);

        Assert.Equal(toDoEntity.MapFromDomainModel(), actual.Value);
    }
}
