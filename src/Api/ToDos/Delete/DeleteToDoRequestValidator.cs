using FastEndpoints;

using FluentValidation;

namespace SourceName.Api.ToDos.Delete;

internal class DeleteToDoRequestValidator : Validator<DeleteToDoRequest>
{
    public DeleteToDoRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
