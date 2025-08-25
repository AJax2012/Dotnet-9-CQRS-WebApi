using System.Net.Http.Json;

using SourceName.Api.ToDos.Create;
using SourceName.Test.Integration.Auth;
using SourceName.Test.Integration.VerifyConfig;
using SourceName.TestUtils.ToDos;

namespace SourceName.Test.Integration.ToDoEndpoints;

public class DeleteToDoEndpointTests : IClassFixture<ApplicationApiFactory>
{
    private readonly HttpClient _client;
    private readonly VerifySettings _verifySettings = VerifyGlobalSettings.GetSettings();

    public DeleteToDoEndpointTests(ApplicationApiFactory factory)
    {
        _client = factory.CreateClient();

        var jwtToken = factory.JwtTokenService
            .GenerateJwtToken(TestingIdentity.GenerateClaimsIdentity());

        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwtToken);
    }

    [Fact]
    public async Task Returns_204_NoContent_WhenToDoDeleted()
    {
        var createdResponse = await _client.PostAsJsonAsync("/api/todos", CreateToDoRequestFaker.Faker.Generate());
        var createdContent = await createdResponse.Content.ReadFromJsonAsync<CreateToDoResponse>();

        var actual = await _client.DeleteAsync($"/api/todos/{createdContent!.Id}");

        await Verify(actual, _verifySettings);
    }

    [Fact]
    public async Task Returns_400_BadRequest_WhenToDoInvalid()
    {
        var actual = await _client.DeleteAsync($"/api/todos/{Guid.Empty}");
        await Verify(actual, _verifySettings);
    }

    [Fact]
    public async Task Returns_404_NotFound_WhenToDoNotFound()
    {
        var actual = await _client.DeleteAsync($"/api/todos/{Guid.NewGuid()}");
        await Verify(actual, _verifySettings);
    }
}
