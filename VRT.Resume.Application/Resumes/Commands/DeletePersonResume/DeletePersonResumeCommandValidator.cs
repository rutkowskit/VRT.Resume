using FluentValidation;

namespace VRT.Resume.Application.Resumes.Commands.DeletePersonResume
{
    public sealed class DeletePersonResumeCommandValidator : AbstractValidator<DeletePersonResumeCommand>
    {
        public DeletePersonResumeCommandValidator()
        {
            RuleFor(v => v.ResumeId).GreaterThan(0);            
        }
    }
}
