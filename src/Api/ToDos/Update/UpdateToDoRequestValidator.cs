using FastEndpoints;

using FluentValidation;

namespace SourceName.Api.ToDos.Update;

internal class UpdateToDoRequestValidator : Validator<UpdateToDoRequest>
{
    public UpdateToDoRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.IsCompleted).NotNull();
        
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(ToDoConstants.TitleMaxLength);
    }
}
