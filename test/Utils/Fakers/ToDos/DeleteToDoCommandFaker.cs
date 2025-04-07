using SourceName.Application.ToDos.Commands;

namespace SourceName.TestUtils.Fakers.ToDos;

public static class DeleteToDoCommandFaker
{
    public static Faker<DeleteToDoCommand> Faker => new AutoFaker<DeleteToDoCommand>()
        .RuleFor(x => x.Id, f => f.Random.Guid())
        .RuleFor(x => x.UserId, f => f.Random.Guid());
}
