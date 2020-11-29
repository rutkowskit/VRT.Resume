using FluentValidation;

namespace VRT.Resume.Application.Persons.Commands.UpsertPersonSkill
{
    public sealed class UpsertPersonSkillCommandValidator : AbstractValidator<UpsertPersonSkillCommand>
    {
        public UpsertPersonSkillCommandValidator()
        {
            RuleFor(v => v.SkillName).MinimumLength(1).NotEmpty();
            RuleFor(v => v.SkillLevel).MinimumLength(1).NotEmpty();            
        }
    }
}
