using FluentValidation;

namespace VRT.Resume.Application.Persons.Commands.MergePersonDutySkills
{
    public sealed class MergePersonDutySkillsCommandValidator : AbstractValidator<MergePersonDutySkillsCommand>
    {
        public MergePersonDutySkillsCommandValidator()
        {
            RuleFor(v => v.DutyId).GreaterThan(0);            
        }
    }
}
