using Serilog;

using SourceName.Application.ToDos.Commands;
using SourceName.Application.ToDos.Contracts;
using SourceName.Application.ToDos.Queries;
using SourceName.Contracts.ToDos;
using SourceName.Domain.ToDos;
using SourceName.TestUtils.ToDos;

namespace SourceName.Test.Application.ToDos.Commands;

public class CreateToDoCommandHandlerTest
{
    private readonly IToDosRepository _toDoRepository = Substitute.For<IToDosRepository>();
    private readonly ILogger _logger = Substitute.For<ILogger>();
    private readonly CreateToDoCommand _createToDoCommand = CreateToDoCommandFaker.Generate();
    private readonly CreateToDoCommandHandler _sut;

    public CreateToDoCommandHandlerTest()
    {
        _sut = new(_toDoRepository, _logger);
    }

    [Fact]
    public async Task ExecuteAsync_ThrowsArgumentNullException_WhenCommandIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.ExecuteAsync(null!, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_CallsGetByTitleAsync_WhenRequestIsNotNull()
    {
        await _sut.ExecuteAsync(_createToDoCommand, CancellationToken.None);

        await _toDoRepository.Received()
            .GetByTitleAsync(
                _createToDoCommand.Title,
                _createToDoCommand.UserId,
                CancellationToken.None);
    }

    [Fact]
    public async Task ExecuteAsync_LogsAndReturnsConflict_WhenToDoAlreadyExists()
    {
        _toDoRepository.GetByTitleAsync(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(ToDoEntityFaker.Generate().First());

        var actual = await _sut.ExecuteAsync(_createToDoCommand, CancellationToken.None);

        _logger.Received()
            .Information(
                "ToDo with toDoTitle {Title} already exists for user {UserId}",
                _createToDoCommand.Title,
                _createToDoCommand.UserId);

        Assert.Single(actual.Errors);
        Assert.Equal(ToDoErrors.Conflict, actual.FirstError);
    }

    [Fact]
    public async Task ExecuteAsync_CallsGetCountAsync_WhenToDoDoesNotExist()
    {
        await _sut.ExecuteAsync(_createToDoCommand, CancellationToken.None);

        await _toDoRepository.Received()
            .GetCountAsync(Arg.Is<GetToDosFilteredQuery>(x =>
                    x.OrderBy == ToDosOrderBy.DisplayOrder.ToStringFast() &&
                    x.IsDescending == true &&
                    x.IsCompleted == false &&
                    x.Ids.Count == 0 &&
                    x.Limit == null &&
                    x.NextPageToken == null &&
                    x.Title == null),
                CancellationToken.None);
    }

    [Fact]
    public async Task ExecuteAsync_CallsCreateAsync_WhenToDoDoesNotExist()
    {
        _toDoRepository.GetCountAsync(Arg.Any<GetToDosFilteredQuery>(), Arg.Any<CancellationToken>())
            .Returns(0);

        await _sut.ExecuteAsync(_createToDoCommand, CancellationToken.None);

        await _toDoRepository.Received()
            .CreateAsync(Arg.Is<ToDoEntity>(x =>
                    x.CreatedByUserId == _createToDoCommand.UserId &&
                    x.Title.Value == _createToDoCommand.Title &&
                    x.Status.DisplayOrder == 1),
                CancellationToken.None);
    }

    [Fact]
    public async Task ExecuteAsync_LogsAndReturnsSqlError_WhenCreateAsyncFails()
    {
        _toDoRepository.CreateAsync(Arg.Any<ToDoEntity>(), Arg.Any<CancellationToken>())
            .Returns(0);

        var actual = await _sut.ExecuteAsync(_createToDoCommand, CancellationToken.None);

        _logger.Received()
            .Error("Failed to create ToDo for user {UserId}", _createToDoCommand.UserId);

        Assert.Single(actual.Errors);
        Assert.Equal(ToDoErrors.SqlError, actual.FirstError);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsToDoId_WhenCreateAsyncSucceeds()
    {
        _toDoRepository.CreateAsync(Arg.Any<ToDoEntity>(), Arg.Any<CancellationToken>())
            .Returns(1);

        var actual = await _sut.ExecuteAsync(_createToDoCommand, CancellationToken.None);

        Assert.Empty(actual.ErrorsOrEmptyList);
        Assert.NotEqual(Guid.Empty, actual.Value);
    }
}
