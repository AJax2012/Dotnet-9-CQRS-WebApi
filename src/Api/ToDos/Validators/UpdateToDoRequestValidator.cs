using FastEndpoints;

using FluentValidation;

using SourceName.Contracts.ToDos;
using SourceName.Contracts.ToDos.Requests;

namespace SourceName.Api.ToDos.Validators;

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
