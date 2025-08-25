using System.Net.Http.Json;

using SourceName.Api.ToDos.Create;
using SourceName.Api.ToDos.Update;
using SourceName.Test.Integration.Auth;
using SourceName.Test.Integration.VerifyConfig;
using SourceName.TestUtils.ToDos;

namespace SourceName.Test.Integration.ToDoEndpoints;

public class UpdateToDoEndpointTests : IClassFixture<ApplicationApiFactory>
{
    private readonly HttpClient _client;
    private readonly VerifySettings _verifySettings = VerifyGlobalSettings.GetSettings();

    public UpdateToDoEndpointTests(ApplicationApiFactory factory)
    {
        _client = factory.CreateClient();

        var jwtToken = factory.JwtTokenService
            .GenerateJwtToken(TestingIdentity.GenerateClaimsIdentity());

        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwtToken);
    }

    [Fact]
    public async Task Returns_204_NoContent_WhenToDoUpdated()
    {
        var createdResponse = await _client.PostAsJsonAsync("/api/todos", CreateToDoRequestFaker.Faker.Generate());
        var createdContent = await createdResponse.Content.ReadFromJsonAsync<CreateToDoResponse>();

        var updateRequest = new UpdateToDoRequest
        {
            Title = "Updated title",
            IsCompleted = false,
        };

        var actual = await _client.PutAsJsonAsync($"/api/todos/{createdContent!.Id}", updateRequest);

        await Verify(actual, _verifySettings);
    }

    [Fact]
    public async Task Returns_400_BadRequest_WhenRequestInvalid()
    {
        var updateRequest = new UpdateToDoRequest
        {
            Title = string.Empty,
            IsCompleted = false,
        };

        var actual = await _client.PutAsJsonAsync($"/api/todos/{Guid.NewGuid()}", updateRequest);

        await Verify(actual, _verifySettings);
    }

    [Fact]
    public async Task Returns_404_NotFound_WhenToDoNotFound()
    {
        var updateRequest = new UpdateToDoRequest
        {
            Title = "Updated toDoTitle",
            IsCompleted = false,
        };

        var actual = await _client.PutAsJsonAsync($"/api/todos/{Guid.NewGuid()}", updateRequest);

        await Verify(actual, _verifySettings);
    }
}
