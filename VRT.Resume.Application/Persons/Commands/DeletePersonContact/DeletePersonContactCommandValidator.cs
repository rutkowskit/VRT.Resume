using FluentValidation;

namespace VRT.Resume.Application.Persons.Commands.DeletePersonContact
{
    public sealed class DeletePersonContactCommandValidator : AbstractValidator<DeletePersonContactCommand>
    {
        public DeletePersonContactCommandValidator()
        {
            RuleFor(v => v.ContactId).GreaterThan(0);            
        }
    }
}
