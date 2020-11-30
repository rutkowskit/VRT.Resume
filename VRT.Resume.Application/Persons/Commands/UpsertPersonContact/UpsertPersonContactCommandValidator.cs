using FluentValidation;

namespace VRT.Resume.Application.Persons.Commands.UpsertPersonContact
{
    public sealed class UpsertPersonContactCommandValidator : AbstractValidator<UpsertPersonContactCommand>
    {
        public UpsertPersonContactCommandValidator()
        {
            RuleFor(v => v.Name).MinimumLength(1).NotEmpty();
            RuleFor(v => v.Value).MinimumLength(1).NotEmpty();            
        }
    }
}
