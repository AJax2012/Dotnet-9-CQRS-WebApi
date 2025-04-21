using System.Net.Http.Json;

using SourceName.Api.ToDos.Create;
using SourceName.Api.ToDos.GetByFilter;
using SourceName.Test.Integration.Auth;
using SourceName.Test.Integration.VerifyConfig;

namespace SourceName.Test.Integration.ToDoEndpoints;

public class GetToDosFilteredEndpointTests : IClassFixture<ApplicationApiFactory>
{
    private readonly HttpClient _client;
    private readonly VerifySettings _verifySettings = VerifyGlobalSettings.GetSettings();

    public GetToDosFilteredEndpointTests(ApplicationApiFactory factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + MockTokenGenerator.GenerateJwtToken());
    }
    
    [Fact]
    public async Task Returns_200_Ok_WhenToDosFound()
    {
        await _client.PostAsJsonAsync("/todos", new CreateToDoRequest { Title = "Test" });
        
        var actual = await _client.GetAsync("/todos?title=Test");
        var content = await actual.Content.ReadFromJsonAsync<ToDosResponse>();
        
        await Verify(actual, _verifySettings)
            .ScrubHttpTextResponse(x => x.Replace(content!.NextPageToken, "nextPageToken"));
    }
    
    [Fact]
    public async Task Returns_200_Ok_WhenToDosFoundWithCursor()
    {
        await _client.PostAsJsonAsync("/todos", new CreateToDoRequest { Title = "Test" });
        
        var actual = await _client.GetAsync("/todos?title=Test&cursor=nextPageToken");
        var content = await actual.Content.ReadFromJsonAsync<ToDosResponse>();
        
        await Verify(actual, _verifySettings)
            .ScrubHttpTextResponse(x => x.Replace(content!.NextPageToken, "nextPageToken"));
    }
    
    [Fact]
    public async Task Returns_404_NotFound_WhenToDosNotFound()
    {
        var actual = await _client.GetAsync("/todos?title=NotFound");
        await Verify(actual, _verifySettings);
    }
}
