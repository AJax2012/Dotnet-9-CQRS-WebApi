using FastEndpoints;

using FluentValidation;

namespace SourceName.Api.ToDos.Create;

internal class CreateToDoRequestValidator : Validator<CreateToDoRequest>
{
    public CreateToDoRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(ToDoConstants.TitleMaxLength);
    }
}
