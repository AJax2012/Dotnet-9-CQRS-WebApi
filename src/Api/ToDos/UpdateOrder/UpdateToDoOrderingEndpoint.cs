using FastEndpoints;

using SourceName.Api.ErrorHandling;
using SourceName.Api.ToDos.ExampleResponses;
using SourceName.Application.ToDos.Commands;

namespace SourceName.Api.ToDos.UpdateOrder;

internal class UpdateToDoOrderingEndpoint : Endpoint<UpdateToDoOrderingRequest>
{
    public override void Configure()
    {
        Put("/order");
        Group<ToDosGroup>();

        Description(x => x
            .Accepts<UpdateToDoOrderingRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblemFE()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithName("UpdateToDoOrdering"));

        Summary(s =>
        {
            s.Summary = "Update orders of multiple To Dos";
            s.Description = "Updates the order of multiple To Dos by passing a dictionary of To Do ids and orders";
            s.ExampleRequest = ToDoRequestExamples.UpdateToDoOrderingRequest;
            s.Responses[404] = ToDoResponseExamples.NotFoundResponse;
            s.Responses[500] = ToDoResponseExamples.SqlErrorResponse;
        });
    }

    public override async Task HandleAsync(UpdateToDoOrderingRequest request, CancellationToken cancellationToken)
    {
        if (ValidationFailed)
        {
            await SendResultAsync(ValidationFailures.ToProblemDetailsResult());
            return;
        }

        var result = await new UpdateToDoOrderingCommand(request.ToDos, request.UserId)
            .ExecuteAsync(cancellationToken);

        await result.Match(
            _ => SendNoContentAsync(cancellationToken),
            errors => SendResultAsync(errors.ToProblemDetailsResult()));
    }
}
