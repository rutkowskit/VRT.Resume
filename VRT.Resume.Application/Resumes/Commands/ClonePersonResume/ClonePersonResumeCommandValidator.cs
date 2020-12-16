using FluentValidation;

namespace VRT.Resume.Application.Resumes.Commands.ClonePersonResume
{
    public sealed class ClonePersonResumeCommandValidator : AbstractValidator<ClonePersonResumeCommand>
    {
        public ClonePersonResumeCommandValidator()
        {
            RuleFor(v => v.ResumeId).GreaterThan(0);            
        }
    }
}
