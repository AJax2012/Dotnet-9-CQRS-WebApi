using SourceName.Domain.ToDos;

namespace SourceName.TestUtils.Fakers.ToDos;

public static class ToDoEntityFaker
{
    public static readonly Guid EntityCreatedByUserId = Guid.Parse("6d2d4c8c-7c4a-4d4b-8c7b-4a3d2c1e1f0e");

    public static List<ToDoEntity> Generate(int count = 1) => 
        Enumerable.Range(1 , count)
            .Select(i => new ToDoEntity(EntityCreatedByUserId, new Faker().Random.Words(), i))
            .ToList();
}
