using FastEndpoints;
using FluentValidation;

using SourceName.Contracts.ToDos.Requests;

namespace SourceName.Api.ToDos.Validators;

internal class DeleteToDoRequestValidator : Validator<DeleteToDoRequest>
{
    public DeleteToDoRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
