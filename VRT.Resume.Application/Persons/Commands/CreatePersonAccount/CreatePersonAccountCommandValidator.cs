using FluentValidation;

namespace VRT.Resume.Application.Persons.Commands.CreatePersonAccount
{
    public sealed class CreatePersonAccountCommandValidator : AbstractValidator<CreatePersonAccountCommand>
    {
        public CreatePersonAccountCommandValidator()
        {
            RuleFor(v => v.Email)
                .EmailAddress()                
                .NotEmpty();

            RuleFor(v => v.FirstName)
                .MaximumLength(50);

            RuleFor(v => v.LastName)
                .MaximumLength(100);                
        }
    }
}
