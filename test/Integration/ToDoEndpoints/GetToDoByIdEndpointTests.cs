using System.Net.Http.Json;

using SourceName.Api.ToDos.Create;
using SourceName.Test.Integration.Auth;
using SourceName.Test.Integration.VerifyConfig;

namespace SourceName.Test.Integration.ToDoEndpoints;

public class GetToDoByIdEndpointTests : IClassFixture<ApplicationApiFactory>
{
    private readonly HttpClient _client;
    private readonly VerifySettings _verifySettings = VerifyGlobalSettings.GetSettings();
    
    public GetToDoByIdEndpointTests(ApplicationApiFactory factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + MockTokenGenerator.GenerateJwtToken());
    }
    
    [Fact]
    public async Task Returns_200_Ok_WhenToDoFoundAndUserIdMatches()
    {
        var createRequest = new CreateToDoRequest { Title = "Test Title" };
        var createdAtResponse = await _client.PostAsJsonAsync("/todos", createRequest);
        var createdAtContent = await createdAtResponse.Content.ReadFromJsonAsync<CreateToDoResponse>();
        var actual = await _client.GetAsync($"/todos/{createdAtContent!.Id}");
        
        await Verify(actual, _verifySettings);
    }
    
    [Fact]
    public async Task Returns_400_BadRequest_WhenToDoInvalid()
    {
        var actual = await _client.GetAsync($"/todos/{Guid.Empty}");
        await Verify(actual, _verifySettings);
    }
    
    [Fact]
    public async Task Returns_404_NotFound_WhenToDoNotFound()
    {
        var actual = await _client.GetAsync($"/todos/{Guid.NewGuid()}");
        await Verify(actual, _verifySettings);
    }
}
