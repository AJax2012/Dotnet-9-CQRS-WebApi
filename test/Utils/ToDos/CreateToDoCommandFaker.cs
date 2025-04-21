using SourceName.Application.ToDos.Commands;

namespace SourceName.TestUtils.ToDos;

public static class CreateToDoCommandFaker
{
    public static CreateToDoCommand Generate() => new AutoFaker<CreateToDoCommand>()
        .RuleFor(x => x.Title, f => f.Random.Words())
        .RuleFor(x => x.UserId, f => f.Random.Guid())
        .Generate();
}
