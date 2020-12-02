using FluentValidation;

namespace VRT.Resume.Application.Persons.Commands.DeletePersonExperienceDuty
{
    public sealed class DeletePersonExperienceDutyCommandValidator : AbstractValidator<DeletePersonExperienceDutyCommand>
    {
        public DeletePersonExperienceDutyCommandValidator()
        {
            RuleFor(v => v.DutyId).GreaterThan(0);            
        }
    }
}
