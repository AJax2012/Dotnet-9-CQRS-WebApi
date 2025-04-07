using SourceName.Contracts.ToDos.Requests;

namespace SourceName.TestUtils.Fakers.ToDos;

public static class CreateToDoRequestFaker
{
    public static Faker<CreateToDoRequest> Faker => new Faker<CreateToDoRequest>()
        .RuleFor(x => x.Title, f => f.Random.Words())
        .RuleFor(x => x.UserId, f => f.Random.Guid());
}
