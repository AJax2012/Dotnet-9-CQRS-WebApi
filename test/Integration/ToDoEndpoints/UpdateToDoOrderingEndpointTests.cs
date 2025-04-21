using System.Net.Http.Json;

using SourceName.Api.ToDos.Create;
using SourceName.Api.ToDos.UpdateOrder;
using SourceName.Test.Integration.Auth;
using SourceName.Test.Integration.VerifyConfig;
using SourceName.TestUtils.ToDos;

namespace SourceName.Test.Integration.ToDoEndpoints;

public class UpdateToDoOrderingEndpointTests : IClassFixture<ApplicationApiFactory>
{
    private readonly HttpClient _client;
    private readonly VerifySettings _verifySettings = VerifyGlobalSettings.GetSettings();

    public UpdateToDoOrderingEndpointTests(ApplicationApiFactory factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + MockTokenGenerator.GenerateJwtToken());
    }
    
    [Fact]
    public async Task Returns_204_NoContent_WhenToDoOrderingUpdated()
    {
        var createRequests = CreateToDoRequestFaker.Faker.Generate(2);
        var createdResponse1 = await _client.PostAsJsonAsync("/todos", createRequests[0]);
        var createdContent1 = await createdResponse1.Content.ReadFromJsonAsync<CreateToDoResponse>();
        
        var createdResponse2 = await _client.PostAsJsonAsync("/todos", createRequests[1]);
        var createdContent2 = await createdResponse2.Content.ReadFromJsonAsync<CreateToDoResponse>();

        var updateToDoOrderingRequest = new UpdateToDoOrderingRequest
        {
            ToDos = new()
            {
                { createdContent1!.Id, 2 },
                { createdContent2!.Id, 1 }
            }
        };
        
        var actual = await _client.PutAsJsonAsync("/todos/order", updateToDoOrderingRequest);
        
        await Verify(actual, _verifySettings);
    }
    
    [Fact]
    public async Task Returns_400_BadRequest_WhenToDoOrderingInvalid()
    {
        var updateToDoOrderingRequest = new UpdateToDoOrderingRequest
        {
            ToDos = []
        };
        
        var actual = await _client.PutAsJsonAsync("/todos/order", updateToDoOrderingRequest);
        
        await Verify(actual, _verifySettings);
    }
    
    [Fact]
    public async Task Returns_404_NotFound_WhenToDosNotFound()
    {
        var updateToDoOrderingRequest = new UpdateToDoOrderingRequest
        {
            ToDos = new()
            {
                { Guid.NewGuid(), 1 }
            }
        };
        
        var actual = await _client.PutAsJsonAsync("/todos/order", updateToDoOrderingRequest);
        
        await Verify(actual, _verifySettings);
    }
}
