using FastEndpoints;

using FluentValidation;

using SourceName.Contracts.ToDos;

namespace SourceName.Api.ToDos.GetByFilter;

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
