using FastEndpoints;

using SourceName.Api.Extensions;
using SourceName.Application.ToDos.Queries;
using SourceName.Contracts.ToDos.Examples;
using SourceName.Contracts.ToDos.Requests;
using SourceName.Contracts.ToDos.Responses;

namespace SourceName.Api.ToDos.Endpoints;

internal class GetToDoByIdEndpoint : Endpoint<GetToDoByIdRequest, ToDoResource>
{
    public override void Configure()
    {
        Get("/{id:guid}");
        Group<ToDosGroup>();
        
        Description(x => x
            .Accepts<GetToDoByIdRequest>()
            .Produces<ToDoResource>(StatusCodes.Status200OK, "application/json")
            .ProducesProblemFE()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithName("GetToDoById"));
        
        Summary(s =>
        {
            s.Summary = "Get a ToDo by id";
            s.Description = "Get a ToDo by id";
            s.ExampleRequest = ToDoRequestExamples.GetToDoByIdRequest;
            s.ResponseExamples[200] = ToDoResponseExamples.ToDoResource;
            s.Responses[404] = ToDoResponseExamples.NotFoundResponse;
        });
    }
    
    public override async Task HandleAsync(GetToDoByIdRequest request, CancellationToken cancellationToken)
    {
        if (ValidationFailed)
        {
            await SendErrorsAsync(StatusCodes.Status400BadRequest, cancellationToken);
            return;
        }
        
        ArgumentNullException.ThrowIfNull(request);
        var result = await new GetToDoByIdQuery(request.Id, request.UserId)
            .ExecuteAsync(cancellationToken);
        
        await result.Match(
            entity => SendOkAsync(entity.MapToResponse(), cancellationToken),
            errors => SendResultAsync(errors.ToProblemDetailsResult()));
    }
}
