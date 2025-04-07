using FastEndpoints;

using FluentValidation;

using SourceName.Contracts.ToDos;
using SourceName.Contracts.ToDos.Requests;

namespace SourceName.Api.ToDos.Validators;

internal class GetToDosFilteredRequestValidator : Validator<GetToDosFilteredRequest>
{
    public GetToDosFilteredRequestValidator()
    {
        RuleFor(x => x.OrderBy)
            .IsEnumName(typeof(ToDosOrderBy), false);
        
        RuleFor(x => x.Limit)
            .NotNull()
            .GreaterThanOrEqualTo(5)
            .LessThanOrEqualTo(100);
    }
}
