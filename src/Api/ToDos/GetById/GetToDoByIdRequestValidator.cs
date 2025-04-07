using FastEndpoints;

using FluentValidation;

namespace SourceName.Api.ToDos.GetById;

internal class GetToDoByIdRequestValidator : Validator<GetToDoByIdRequest>
{
    public GetToDoByIdRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
