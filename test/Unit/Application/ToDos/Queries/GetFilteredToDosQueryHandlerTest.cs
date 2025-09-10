using MELT;
using MELT.Xunit;
using Microsoft.Extensions.Logging;
using SourceName.Application.ToDos.Contracts;
using SourceName.Application.ToDos.Models;
using SourceName.Application.ToDos.Queries;
using SourceName.Domain.ToDos;
using SourceName.TestUtils.ToDos;

namespace SourceName.Test.Application.ToDos.Queries;

public class GetFilteredToDosQueryHandlerTest
{
    private readonly IToDosRepository _toDoRepository = Substitute.For<IToDosRepository>();
    private readonly ITestLoggerFactory _loggerFactory = TestLoggerFactory.Create();
    private readonly ILogger<GetFilteredToDosQueryHandler> _logger;
    private readonly GetFilteredToDosQueryHandler _sut;

    public GetFilteredToDosQueryHandlerTest()
    {
        _logger = _loggerFactory.CreateLogger<GetFilteredToDosQueryHandler>();
        _sut = new(_toDoRepository, _loggerFactory);
    }

    [Fact]
    public async Task ExecuteAsync_ThrowsArgumentNullException_WhenRequestIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.ExecuteAsync(null!, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_CallsGetFilteredAsync_WhenRequestIsNotNull()
    {
        var request = GetToDosFilteredQueryFaker.Faker.Generate();
        await _sut.ExecuteAsync(request, CancellationToken.None);

        await _toDoRepository.Received()
            .GetFilteredAsync(Arg.Is<GetToDosFilteredQuery>(x =>
                    x.OrderBy == request.OrderBy &&
                    x.IsDescending == request.IsDescending &&
                    x.Ids.SequenceEqual(request.Ids) &&
                    x.Limit == request.Limit &&
                    x.NextPageToken == request.NextPageToken &&
                    x.Title == request.Title &&
                    x.IsCompleted == request.IsCompleted
                ),
                null,
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_CallsGetFilteredAsync_WithCursor_WhenRequestHasCursor()
    {
        var cursorEntity = ToDoEntityFaker.Generate().First();
        var request = GetToDosFilteredQueryFaker.GenerateWithNextPageToken(cursorEntity);
        await _sut.ExecuteAsync(request, CancellationToken.None);

        await _toDoRepository.Received()
            .GetFilteredAsync(Arg.Is<GetToDosFilteredQuery>(x =>
                    x.OrderBy == request.OrderBy &&
                    x.IsDescending == request.IsDescending &&
                    x.Ids.SequenceEqual(request.Ids) &&
                    x.Limit == request.Limit &&
                    x.NextPageToken == request.NextPageToken &&
                    x.Title == request.Title &&
                    x.IsCompleted == request.IsCompleted
                ),
                Arg.Is<ToDoEntity>(x =>
                    x.Id == cursorEntity.Id &&
                    x.Status.DisplayOrder == cursorEntity.Status.DisplayOrder &&
                    x.Title.Value == cursorEntity.Title.Value),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_LogsAndReturnsNotFound_WhenToDosNotFound()
    {
        var actual = await _sut.ExecuteAsync(GetToDosFilteredQueryFaker.Faker.Generate(), CancellationToken.None);

        var log = Assert.Single(_loggerFactory.Sink.LogEntries);
        Assert.Equal(LogLevel.Information, log.LogLevel);
        Assert.Equal("No ToDos found", log.Message);

        Assert.Single(actual.ErrorsOrEmptyList);
        Assert.Equal(ToDoErrors.NotFound, actual.FirstError);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ExecuteAsync_ReturnsToDos_WhenToDosFound(bool hasNextPage)
    {
        var request = GetToDosFilteredQueryFaker.Faker.Generate();
        var numberToGenerate = hasNextPage ? request.Limit!.Value + 1 : request.Limit!.Value;
        var toDos = ToDoEntityFaker.Generate(numberToGenerate);
        var expectedNextPageToken = ToDoNextResultToken.EncodeToken(toDos.Last());

        _toDoRepository.GetFilteredAsync(Arg.Any<GetToDosFilteredQuery>(), null, Arg.Any<CancellationToken>())
            .Returns(toDos);

        var expected = new List<ToDo>();

        if (hasNextPage)
        {
            expected.AddRange(
                toDos
                    .Take(request.Limit!.Value)
                    .Select(x => x.MapFromEntity()));
        }
        else
        {
            expected.AddRange(toDos.Select(x =>
                x.MapFromEntity()));
        }

        var actual = await _sut.ExecuteAsync(GetToDosFilteredQueryFaker.Faker.Generate(), CancellationToken.None);

        Assert.Empty(actual.ErrorsOrEmptyList);
        Assert.Equal(hasNextPage, actual.Value.HasNextPage);
        Assert.Equal(expectedNextPageToken, actual.Value.NextPageToken);

        Assert.Equal(expected.Count, actual.Value.Items.Count);

        foreach (var item in actual.Value.Items)
        {
            Assert.Contains(item, expected);
        }
    }
}
