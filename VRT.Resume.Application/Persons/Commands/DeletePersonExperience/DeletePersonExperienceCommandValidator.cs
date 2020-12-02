using FluentValidation;

namespace VRT.Resume.Application.Persons.Commands.DeletePersonExperience
{
    public sealed class DeletePersonExperienceCommandValidator : AbstractValidator<DeletePersonExperienceCommand>
    {
        public DeletePersonExperienceCommandValidator()
        {
            RuleFor(v => v.ExperienceId).GreaterThan(0);            
        }
    }
}
