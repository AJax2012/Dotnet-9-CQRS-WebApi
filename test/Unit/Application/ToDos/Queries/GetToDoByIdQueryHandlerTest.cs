using Serilog;

using SourceName.Application.ToDos.Contracts;
using SourceName.Application.ToDos.Models;
using SourceName.Application.ToDos.Queries;
using SourceName.Domain.ToDos;
using SourceName.TestUtils.ToDos;

namespace SourceName.Test.Application.ToDos.Queries;

public class GetToDoByIdQueryHandlerTest
{
    private readonly IToDosRepository _toDoRepository = Substitute.For<IToDosRepository>();
    private readonly ILogger _logger = Substitute.For<ILogger>();
    private readonly GetToDoByIdQueryHandler _sut;

    public GetToDoByIdQueryHandlerTest()
    {
        _sut = new GetToDoByIdQueryHandler(_toDoRepository, _logger);
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

        _logger.Received()
            .Warning("Todo with id {Id} not found", request.Id);

        Assert.Single(actual.Errors);
        Assert.Equal(ToDoErrors.NotFound, actual.FirstError);
    }

    [Fact]
    public async Task ExecuteAsync_LogsAndReturnsNotFound_WhenToDoFoundButUserIdDoesNotMatch()
    {
        var toDoEntity = ToDoEntityFaker.Generate().First();
        var request = GetToDoByIdQueryFaker.Faker.Generate();

        _toDoRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(toDoEntity);

        var actual = await _sut.ExecuteAsync(request, CancellationToken.None);

        _logger.Received()
            .Warning(
                "Todo with id {Id} does not belong to user {UserId}",
                request.Id,
                request.UserId);

        Assert.Single(actual.Errors);
        Assert.Equal(ToDoErrors.NotFound, actual.FirstError);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsToDo_WhenToDoFoundAndUserIdMatches()
    {
        var toDoEntity = ToDoEntityFaker.Generate().First();

        var request = GetToDoByIdQueryFaker.Faker
            .RuleFor(x => x.UserId, toDoEntity.CreatedByUserId)
            .RuleFor(x => x.Id, toDoEntity.Id)
            .Generate();

        _toDoRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(toDoEntity);

        var actual = await _sut.ExecuteAsync(request, CancellationToken.None);

        Assert.Equal(toDoEntity.MapFromEntity(), actual.Value);
    }
}
