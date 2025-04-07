using SourceName.Domain.ToDos;
using SourceName.TestUtils.Fakers.ToDos;

namespace SourceName.Test.Domain.ToDos;

public class ToDoEntityTests
{
    [Fact]
    public void Constructor_PropertiesAreSet_WhenThreeParametersProvided()
    {
        var createdByUserId = Guid.NewGuid();
        var title = new Faker().Random.Words();
        var order = new Faker().Random.Int();
        
        var sut = new ToDoEntity(createdByUserId, title, order);
        
        Assert.Equal(createdByUserId, sut.CreatedByUserId);
        Assert.Equal(title, sut.Title);
        Assert.Equal(order, sut.DisplayOrder);
        Assert.False(sut.IsCompleted);
        Assert.NotEqual(sut.Id, Guid.Empty);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Constructor_PropertiesAreSet_WhenAllParametersProvided(bool includeOrder)
    {
        var id = Guid.NewGuid();
        var createdByUserId = Guid.NewGuid();
        var title = new Faker().Random.Words();
        var isCompleted = new Faker().Random.Bool();
        var order = includeOrder ? new Faker().Random.Int() : (int?)null;
        var createdAt = new Faker().Date.Past();
        var updatedAt = new Faker().Date.Past();

        var sut = new ToDoEntity(id, createdByUserId, title, isCompleted, createdAt, updatedAt, order);
        
        Assert.Equal(id, sut.Id);
        Assert.Equal(createdByUserId, sut.CreatedByUserId);
        Assert.Equal(title, sut.Title);
        Assert.Equal(isCompleted, sut.IsCompleted);
        Assert.Equal(createdAt, sut.CreatedAt);
        Assert.Equal(updatedAt, sut.UpdatedAt);
        Assert.Equal(order, sut.DisplayOrder);
    }
    
    [Fact]
    public void Update_SetsTitle_IsCompleted_AndUpdatedAt_WhenTitleAndIsCompletedProvided()
    {
        var sut = ToDoEntityFaker.Generate().First();
        var originalUpdatedAt = sut.UpdatedAt;
        var title = new Faker().Random.Words();
        var isCompleted = !sut.IsCompleted;
        
        sut.Update(title, isCompleted);
        
        Assert.Equal(title, sut.Title);
        Assert.Equal(isCompleted, sut.IsCompleted);
        Assert.NotEqual(originalUpdatedAt, sut.UpdatedAt);
    }
    
    [Fact]
    public void Update_SetsOrderToNull_WhenIsCompletedIsTrue()
    {
        var sut = ToDoEntityFaker.Generate().First();
        sut.Update(sut.Title, true);
        Assert.Null(sut.DisplayOrder);
    }
    
    [Fact]
    public void UpdateOrder_DoesNotSetOrder_WhenIsCompletedIsTrue()
    {
        var sut = ToDoEntityFaker.Generate().First();
        sut.Update(sut.Title, true);
        sut.UpdateOrder(new Faker().Random.Int());
        Assert.Null(sut.DisplayOrder);
    }
    
    [Fact]
    public void UpdateOrder_OrderIsUpdated_WhenNewOrderProvidedAndIsCompletedIsFalse()
    {
        var sut = ToDoEntityFaker.Generate().First();
        sut.Update(sut.Title, false);
        var order = new Faker().Random.Int();
        sut.UpdateOrder(order);
        Assert.Equal(order, sut.DisplayOrder);
    }
}
