using FluentValidation;

namespace VRT.Resume.Application.Persons.Commands.CreatePersonAccount
{
    public sealed class CreatePersonAccountCommandValidator : AbstractValidator<CreatePersonAccountCommand>
    {
        public CreatePersonAccountCommandValidator()
        {
            RuleFor(v => v.UserId)                
                .NotEmpty();

            RuleFor(v => v.Email)
                .EmailAddress();

            RuleFor(v => v.FirstName)                
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(v => v.LastName)
                .MaximumLength(100);                
        }
    }
}
