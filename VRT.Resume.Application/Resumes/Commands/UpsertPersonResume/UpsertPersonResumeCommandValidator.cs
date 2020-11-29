using FluentValidation;

namespace VRT.Resume.Application.Resumes.Commands.UpsertPersonResume
{
    public sealed class UpsertPersonResumeCommandValidator : AbstractValidator<UpsertPersonResumeCommand>
    {
        public UpsertPersonResumeCommandValidator()
        {
            RuleFor(v => v.Description).MinimumLength(1).NotEmpty();
            RuleFor(v => v.Position).MinimumLength(1).NotEmpty();            
        }
    }
}
