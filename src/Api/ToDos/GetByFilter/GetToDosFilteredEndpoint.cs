using FastEndpoints;

using SourceName.Api.ErrorHandling;
using SourceName.Api.ToDos.ExampleResponses;
using SourceName.Application.ToDos.Queries;

namespace SourceName.Api.ToDos.GetByFilter;

internal class GetToDosFilteredEndpoint : Endpoint<GetToDosFilteredRequest, ToDosResponse>
{
    public override void Configure()
    {
        Get("/");
        Group<ToDosGroup>();

        Description(x => x
            .Accepts<GetToDosFilteredRequest>()
            .Produces<ToDosResponse>(StatusCodes.Status200OK, "application/json")
            .ProducesProblemFE()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithName("GetToDosFiltered"));

        Summary(s =>
        {
            s.Summary = "Get To Dos filtered";
            s.Description = "Get To Dos filtered";
            s.RequestParam(x => x.Limit, "The maximum number of To Dos to return. Must be between 5 and 100. Defaults to 25.");
            s.RequestParam(x => x.OrderBy, "The property to order the filtered To Do results by. Must be either Id or Title. Id also sorts by date created. Defaults to Id.");
            s.RequestParam(x => x.IsDescending, "If true, returns results in descending order. If false, returns results in ascending order. Defaults to false (alphabetic order or in order of creation).");
            s.RequestParam(x => x.Title, "The toDoTitle of the To Do. Is not case sensitive and supports partial matching. Defaults to null (no filtering).");
            s.RequestParam(x => x.IsCompleted, "Returns only completed To Dos if true, and returns only incomplete To Dos if false. Defaults to null (no filtering).");
            s.RequestParam(x => x.NextPageToken, "The token to retrieve the next batch of To Dos. If not provided, returns the first batch of To Dos.");
            s.RequestExamples.Add(new(ToDoRequestExamples.GetToDosFilteredRequestWithAllProperties));
            s.RequestExamples.Add(new(ToDoRequestExamples.GetToDosFilteredRequestWithoutNullableProperties));
            s.ResponseExamples[200] = ToDoResponseExamples.ToDoResource;
            s.Responses[404] = ToDoResponseExamples.NotFoundResponse;
        });
    }

    public override async Task HandleAsync(GetToDosFilteredRequest request, CancellationToken ct)
    {
        if (ValidationFailed)
        {
            await SendResultAsync(ValidationFailures.ToProblemDetailsResult());
            return;
        }

        ArgumentNullException.ThrowIfNull(request);
        var isDesc = request.IsDescending ?? false;

        var result = await new GetToDosFilteredQuery(
            OrderBy: request.OrderBy,
            IsDescending: isDesc,
            [],
            Limit: request.Limit ?? 25,
            NextPageToken: request.NextPageToken,
            Title: request.Title,
            IsCompleted: request.IsCompleted
        ).ExecuteAsync(ct);

        await result.Match(
            dto => SendOkAsync(new(dto.Items.MapToResponse(), dto.HasNextPage, dto.NextPageToken), ct),
            errors => SendResultAsync(errors.ToProblemDetailsResult()));
    }
}
