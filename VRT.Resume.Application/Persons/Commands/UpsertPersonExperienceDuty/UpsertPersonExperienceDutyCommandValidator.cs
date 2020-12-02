using FluentValidation;

namespace VRT.Resume.Application.Persons.Commands.UpsertPersonExperienceDuty
{
    public sealed class UpsertPersonExperienceDutyCommandValidator : AbstractValidator<UpsertPersonExperienceDutyCommand>
    {
        public UpsertPersonExperienceDutyCommandValidator()
        {
            RuleFor(v => v.ExperienceId).GreaterThan(0);
            RuleFor(v => v.Name).MinimumLength(1).NotEmpty();            
        }
    }
}
