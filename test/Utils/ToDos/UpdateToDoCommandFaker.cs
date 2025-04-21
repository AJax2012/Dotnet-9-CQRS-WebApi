using SourceName.Application.ToDos.Commands;

namespace SourceName.TestUtils.ToDos;

public static class UpdateToDoCommandFaker
{
    public static Faker<UpdateToDoCommand> Faker => new AutoFaker<UpdateToDoCommand>()
        .RuleFor(x => x.Id, f => f.Random.Guid())
        .RuleFor(x => x.Title, f => f.Random.Words())
        .RuleFor(x => x.UserId, f => f.Random.Guid())
        .RuleFor(x => x.IsCompleted, f => f.Random.Bool());
}
