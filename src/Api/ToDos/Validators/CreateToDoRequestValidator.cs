using FastEndpoints;
using FluentValidation;

using SourceName.Contracts.ToDos;
using SourceName.Contracts.ToDos.Requests;

namespace SourceName.Api.ToDos.Validators;

internal class CreateToDoRequestValidator : Validator<CreateToDoRequest>
{
    public CreateToDoRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(ToDoConstants.TitleMaxLength);
    }
}
