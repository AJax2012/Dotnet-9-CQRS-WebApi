using FastEndpoints;

using FluentValidation;

namespace SourceName.Api.ToDos.UpdateOrder;

internal class UpdateToDoOrderingRequestValidator : Validator<UpdateToDoOrderingRequest>
{
    public UpdateToDoOrderingRequestValidator()
    {
        RuleFor(x => x.ToDos).NotEmpty();
    }
}
