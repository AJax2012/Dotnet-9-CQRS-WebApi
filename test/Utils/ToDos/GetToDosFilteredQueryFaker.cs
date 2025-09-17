using SourceName.Application.ToDos.Models;
using SourceName.Application.ToDos.Queries;
using SourceName.Domain.ToDos;

namespace SourceName.TestUtils.ToDos;

public static class GetToDosFilteredQueryFaker
{
    public static Faker<GetToDosFilteredQuery> Faker => new AutoFaker<GetToDosFilteredQuery>()
        .RuleFor(x => x.OrderBy, ToDosOrderBy.DisplayOrder.ToStringFast())
        .RuleFor(x => x.IsDescending, f => f.Random.Bool())
        .RuleFor(x => x.Ids, [])
        .RuleFor(x => x.Limit, 10)
        .RuleFor(x => x.NextPageToken, string.Empty)
        .RuleFor(x => x.Title, f => f.Random.Words())
        .RuleFor(x => x.IsCompleted, f => f.Random.Bool());

    public static GetToDosFilteredQuery GenerateWithNextPageToken(ToDo cursor)
    {
        var cursorToken = ToDoNextResultToken.EncodeToken(cursor);

        return Faker
            .RuleFor(x => x.NextPageToken, cursorToken)
            .Generate();
    }
}
