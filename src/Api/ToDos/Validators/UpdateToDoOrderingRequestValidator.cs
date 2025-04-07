using FastEndpoints;

using FluentValidation;

using SourceName.Contracts.ToDos;
using SourceName.Contracts.ToDos.Requests;

namespace SourceName.Api.ToDos.Validators;

internal class UpdateToDoOrderingRequestValidator : Validator<UpdateToDoOrderingRequest>
{
    public UpdateToDoOrderingRequestValidator()
    {
        RuleFor(x => x.ToDos).NotEmpty();
    }
}
