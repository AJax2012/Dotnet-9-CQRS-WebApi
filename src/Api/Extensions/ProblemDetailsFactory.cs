using System.Text.Json;

using ErrorOr;
using FluentValidation.Results;

namespace SourceName.Api.Extensions;

internal static class ProblemDetailsFactory
{
    /// <summary>
    /// Creates a Problem Details result from a list of <see cref="Error"/>
    /// </summary>
    /// <param name="errors">List of <see cref="Error"/></param>
    /// <returns><see cref="IResult"/></returns>
    /// <exception cref="InvalidOperationException" />
    internal static IResult ToProblemDetailsResult(this List<Error> errors)
    {
        if (errors.Count == 0)
        {
            throw new InvalidOperationException("No errors were found.");
        }
        
        var firstError = errors[0];
        
        return Results.Problem(
            title: firstError.Code,
            detail: firstError.Description,
            type: GetType(firstError),
            statusCode: GetStatusCode(firstError),
            extensions: GetErrors(errors)
        );
    }

    /// <summary>
    /// Creates a Problem Details result from a list of <see cref="ValidationFailure"/>
    /// </summary>
    /// <param name="errors">List of <see cref="ValidationFailure"/></param>
    /// <returns><see cref="IResult"/></returns>
    /// <exception cref="InvalidOperationException" />
    internal static IResult ToProblemDetailsResult(this List<ValidationFailure> errors)
    {
        if (errors.Count == 0)
        {
            throw new InvalidOperationException("No validation errors were found.");
        }
        
        return Results.Problem(
            title: "Invalid Request",
            detail: "The request was invalid",
            type: "https://tools.ietf.org/html/rfc9110#name-400-bad-request",
            statusCode: StatusCodes.Status400BadRequest,
            extensions: GetErrors(errors)
        );
    }

    static string GetType(Error error) =>
        error.Type switch
        {
            ErrorType.Conflict => "https://tools.ietf.org/html/rfc9110#name-409-conflict",
            ErrorType.NotFound => "https://tools.ietf.org/html/rfc9110#name-404-not-found",
            ErrorType.Validation => "https://tools.ietf.org/html/rfc9110#name-400-bad-request",
            _ => "https://tools.ietf.org/html/rfc9110#name-500-internal-server-error"
        };

    static int GetStatusCode(Error error) =>
        error.Type switch
        {
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

    private static Dictionary<string, object?>? GetErrors(List<Error> errors)
    {
        if (errors[0].Type != ErrorType.Validation)
        {
            return null;
        }

        return new()
        {
            { "errors", errors.ToDictionary(x => x.Code, x => x.Description) }
        };
    }

    private static Dictionary<string, object?>? GetErrors(List<ValidationFailure> errors)
    {
        return new()
        {
            {
                "errors",
                errors.GroupBy(x => x.PropertyName)
                    .Select(group => new
                    {
                        Field = group.Key,
                        Errors = group.Select(error => error.ErrorMessage)
                    })
            }
        };
    }
}
