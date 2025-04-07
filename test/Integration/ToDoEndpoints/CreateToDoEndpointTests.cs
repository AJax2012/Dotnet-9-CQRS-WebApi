using System.Net.Http.Json;

using SourceName.Api.ToDos.Create;
using SourceName.Test.Integration.Auth;
using SourceName.Test.Integration.VerifyConfig;
using SourceName.TestUtils.Fakers.ToDos;

namespace SourceName.Test.Integration.ToDoEndpoints;

public class CreateToDoEndpointTests : IClassFixture<ApplicationApiFactory>
{
    private readonly HttpClient _client;
    private readonly VerifySettings _verifySettings = VerifyGlobalSettings.GetSettings();

    public CreateToDoEndpointTests(ApplicationApiFactory factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + MockTokenGenerator.GenerateJwtToken());
    }

    [Fact]
    public async Task Returns_201_Created_WhenToDoCreated()
    {
        var actual = await _client.PostAsJsonAsync("/todos", CreateToDoRequestFaker.Faker.Generate());
        await Verify(actual, _verifySettings);
    }
    
    [Fact]
    public async Task Returns_400_BadRequest_WhenToDoInvalid()
    {
        var actual = await _client.PostAsJsonAsync("/todos", new CreateToDoRequest { Title = "" });
        await Verify(actual, _verifySettings);
    }

    [Fact]
    public async Task Returns_409_Conflict_WhenToDoAlreadyExists()
    {
        var toDo = CreateToDoRequestFaker.Faker.Generate();
        await _client.PostAsJsonAsync("/todos", toDo);
        var actual = await _client.PostAsJsonAsync("/todos", toDo);
        await Verify(actual, _verifySettings);
    }
}
