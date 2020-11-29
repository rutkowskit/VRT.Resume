using FluentValidation;

namespace VRT.Resume.Application.Persons.Commands.UpsertPersonData
{
    public sealed class 
        UpsertPersonDataCommandValidator : AbstractValidator<UpsertPersonDataCommand>
    {
        public UpsertPersonDataCommandValidator()
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
