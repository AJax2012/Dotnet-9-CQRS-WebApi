using SourceName.Application.ToDos.Queries;

namespace SourceName.TestUtils.ToDos;

public static class GetToDoByIdQueryFaker
{
    public static Faker<GetToDoByIdQuery> Faker => new AutoFaker<GetToDoByIdQuery>()
        .RuleFor(x => x.Id, f => f.Random.Guid())
        .RuleFor(x => x.UserId, f => f.Random.Guid());
}
