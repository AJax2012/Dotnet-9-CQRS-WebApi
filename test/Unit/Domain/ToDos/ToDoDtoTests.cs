using SourceName.Domain.ToDos;
using SourceName.TestUtils.ToDos;

namespace SourceName.Test.Domain.ToDos;

public class ToDoDtoTests
{
    [Fact]
    public void Constructor_PropertiesAreSet_WhenThreeParametersProvided()
    {
        var createdByUserId = Guid.NewGuid();
        var title = new Faker().Random.Words();
        var order = new Faker().Random.Int();

        var sut = new ToDo(createdByUserId, new(title), order);

        Assert.Equal(createdByUserId, sut.CreatedByUserId);
        Assert.Equal(title, sut.Title.Value);
        Assert.Equal(order, sut.Status.DisplayOrder);
        Assert.False(sut.Status.IsCompleted);
        Assert.NotEqual(sut.Id, Guid.Empty);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(false, false)]
    public void Constructor_PropertiesAreSet_WhenAllParametersProvided(bool includeOrder, bool isCompleted)
    {
        var id = Guid.NewGuid();
        var createdByUserId = Guid.NewGuid();
        var title = new Faker().Random.Words();
        var displayOrder = includeOrder ? new Faker().Random.Int() : (int?)null;
        var createdAt = new Faker().Date.Past();
        var updatedAt = new Faker().Date.Past();

        var sut = new ToDo(id, createdByUserId, new(title), new ToDoStatus(isCompleted, displayOrder), createdAt, updatedAt);

        Assert.Equal(id, sut.Id);
        Assert.Equal(createdByUserId, sut.CreatedByUserId);
        Assert.Equal(title, sut.Title.Value);
        Assert.Equal(isCompleted, sut.Status.IsCompleted);
        Assert.Equal(createdAt, sut.CreatedAt);
        Assert.Equal(updatedAt, sut.UpdatedAt);
        Assert.Equal(isCompleted ? null : displayOrder, sut.Status.DisplayOrder);
    }

    [Fact]
    public void Update_SetsTitle_IsCompleted_AndUpdatedAt_WhenTitleAndIsCompletedProvided()
    {
        var sut = ToDoFaker.Generate().First();
        var originalUpdatedAt = sut.UpdatedAt;
        var title = new Faker().Random.Words();
        var isCompleted = !sut.Status.IsCompleted;

        sut.Update(title, isCompleted);

        Assert.Equal(title, sut.Title.Value);
        Assert.Equal(isCompleted, sut.Status.IsCompleted);
        Assert.NotEqual(originalUpdatedAt, sut.UpdatedAt);
    }

    [Fact]
    public void Update_SetsOrderToNull_WhenIsCompletedIsTrue()
    {
        var sut = ToDoFaker.Generate().First();
        sut.Update(sut.Title.Value, true);
        Assert.Null(sut.Status.DisplayOrder);
    }

    [Fact]
    public void UpdateOrder_DoesNotSetOrder_WhenIsCompletedIsTrue()
    {
        var sut = ToDoFaker.Generate().First();
        sut.Update(sut.Title.Value, true);
        sut.UpdateOrder(new Faker().Random.Int());
        Assert.Null(sut.Status.DisplayOrder);
    }

    [Fact]
    public void UpdateOrder_OrderIsUpdated_WhenNewOrderProvidedAndIsCompletedIsFalse()
    {
        var sut = ToDoFaker.Generate().First();
        sut.Update(sut.Title.Value, false);
        var order = new Faker().Random.Int();
        sut.UpdateOrder(order);
        Assert.Equal(order, sut.Status.DisplayOrder);
    }
}
