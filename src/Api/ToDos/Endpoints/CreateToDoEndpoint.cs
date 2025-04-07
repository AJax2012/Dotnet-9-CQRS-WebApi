using FastEndpoints;

using SourceName.Api.Extensions;
using SourceName.Application.ToDos.Commands;
using SourceName.Contracts.ToDos.Examples;
using SourceName.Contracts.ToDos.Requests;
using SourceName.Contracts.ToDos.Responses;

namespace SourceName.Api.ToDos.Endpoints;

internal class CreateToDoEndpoint : Endpoint<CreateToDoRequest, CreateToDoResponse>
{
    public override void Configure()
    {
        Post("/");
        Group<ToDosGroup>();
        
        Description(x => x
                .Accepts<CreateToDoRequest>("application/json")
                .Produces<CreateToDoResponse>(StatusCodes.Status201Created, "application/json")
                .ProducesProblemFE()
                .ProducesProblem(StatusCodes.Status409Conflict)
                .ProducesProblem(StatusCodes.Status500InternalServerError)
                .WithName("CreateToDo"));
        
        Summary(s =>
        {
            s.Summary = "Creates a new To Do";
            s.Description = "Creates a new To Do";
            s.ExampleRequest = ToDoRequestExamples.CreateToDoRequest;
            s.ResponseExamples[201] = ToDoResponseExamples.CreateToDoResponse;
            s.Responses[409] = "ToDo with Title already exists.";
            s.Responses[500] = ToDoResponseExamples.SqlErrorResponse;
        });
    }
    
    public override async Task HandleAsync(CreateToDoRequest request, CancellationToken ct)
    {
        if (ValidationFailed)
        {
            await SendErrorsAsync(StatusCodes.Status400BadRequest, ct);
            return;
        }

        ArgumentNullException.ThrowIfNull(request);
        var result = await new CreateToDoCommand(request.UserId, request.Title!)
            .ExecuteAsync(ct);
        
        await result.Match(
            id => SendCreatedAtAsync<GetToDoByIdEndpoint>(new CreateToDoResponse(id), new(id), cancellation: ct),
            errors => SendResultAsync(errors.ToProblemDetailsResult()));
    }
}
