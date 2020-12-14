using FluentValidation;

namespace VRT.Resume.Application.Resumes.Commands.MergeResumeSkills
{
    public sealed class MergeResumeSkillsCommandValidator : AbstractValidator<MergeResumeSkillsCommand>
    {
        public MergeResumeSkillsCommandValidator()
        {
            RuleFor(v => v.ResumeId).GreaterThan(0);            
        }
    }
}
