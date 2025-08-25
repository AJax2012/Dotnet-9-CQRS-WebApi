using FastEndpoints;

using SourceName.Api.ErrorHandling;
using SourceName.Api.ToDos.ExampleResponses;
using SourceName.Application.ToDos.Commands;

namespace SourceName.Api.ToDos.Delete;

internal class DeleteToDoEndpoint : Endpoint<DeleteToDoRequest>
{

    public override void Configure()
    {
        Delete("/{id:guid}");
        Group<ToDosGroup>();

        Description(x => x
            .Accepts<DeleteToDoRequest>()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblemFE()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithName("DeleteToDo"));

        Summary(s =>
        {
            s.Summary = "Delete a ToDo by id";
            s.Description = "Delete a ToDo by id";
            s.ExampleRequest = ToDoRequestExamples.DeleteToDoRequest;
            s.Responses[404] = ToDoResponseExamples.NotFoundResponse;
            s.Responses[500] = ToDoResponseExamples.SqlErrorResponse;
        });
    }

    public override async Task HandleAsync(DeleteToDoRequest request, CancellationToken cancellationToken)
    {
        if (ValidationFailed)
        {
            await SendResultAsync(ValidationFailures.ToProblemDetailsResult());
            return;
        }

        ArgumentNullException.ThrowIfNull(request);
        var result = await new DeleteToDoCommand(request.Id, request.UserId)
            .ExecuteAsync(cancellationToken);

        await result.Match(
            _ => SendNoContentAsync(cancellationToken),
            errors => SendResultAsync(errors.ToProblemDetailsResult()));
    }
}
