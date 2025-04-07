using FastEndpoints;

using FluentValidation;

using SourceName.Contracts.ToDos.Requests;

namespace SourceName.Api.ToDos.Validators;

internal class GetToDoByIdRequestValidator : Validator<GetToDoByIdRequest>
{
    public GetToDoByIdRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
