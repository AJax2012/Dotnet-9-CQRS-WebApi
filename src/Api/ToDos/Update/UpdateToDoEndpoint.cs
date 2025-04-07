using FastEndpoints;

using SourceName.Api.Extensions;
using SourceName.Api.ToDos.ExampleResponses;
using SourceName.Application.ToDos.Commands;

namespace SourceName.Api.ToDos.Update;

internal class UpdateToDoEndpoint : Endpoint<UpdateToDoRequest, ToDoResource>
{
    public override void Configure()
    {
        Put("/{id:guid}");
        Group<ToDosGroup>();
        
        Description(x => x
            .Accepts<UpdateToDoRequest>("application/json")
            .Produces<ToDoResource>(StatusCodes.Status200OK, "application/json")
            .ProducesProblemFE()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithName("UpdateToDo"));
        
        Summary(s =>
        {
            s.Summary = "Update a ToDo by id";
            s.Description = "Update a ToDo by id";
            s.ExampleRequest = ToDoRequestExamples.UpdateToDoRequest;
            s.ResponseExamples[200] = ToDoResponseExamples.ToDoResource;
            s.Responses[404] = ToDoResponseExamples.NotFoundResponse;
            s.Responses[500] = ToDoResponseExamples.SqlErrorResponse;
        });
    }
    
    public override async Task HandleAsync(UpdateToDoRequest request, CancellationToken cancellationToken)
    {
        if (ValidationFailed)
        {
            await SendErrorsAsync(StatusCodes.Status400BadRequest, cancellationToken);
            return;
        }
        
        ArgumentNullException.ThrowIfNull(request);
        var result = await new UpdateToDoCommand(request.Id, request.UserId, request.Title, request.IsCompleted)
            .ExecuteAsync(cancellationToken);
        
        await result.Match(
            entity => SendOkAsync(entity.MapToResponse(), cancellationToken),
            errors => SendResultAsync(errors.ToProblemDetailsResult()));
    }
}
