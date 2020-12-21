using FluentValidation;

namespace VRT.Resume.Application.Persons.Commands.UpdatePersonData
{
    public sealed class 
        UpdatePersonDataCommandValidator : AbstractValidator<UpdatePersonDataCommand>
    {
        public UpdatePersonDataCommandValidator()
        {

            RuleFor(v => v.FirstName)
                .MaximumLength(50)
                .NotEmpty();

            RuleFor(v => v.LastName)
                .MaximumLength(100)
                .NotEmpty();
        }
    }
}
