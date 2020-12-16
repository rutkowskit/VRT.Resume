using FluentValidation;

namespace VRT.Resume.Application.Resumes.Commands.UpsertPersonResume
{
    public sealed class UpsertPersonResumeCommandValidator : AbstractValidator<UpsertPersonResumeCommand>
    {
        public UpsertPersonResumeCommandValidator()
        {
            RuleFor(v => v.Description).NotEmpty().MinimumLength(1).MaximumLength(50);
            RuleFor(v => v.Position).NotEmpty().MinimumLength(1).MaximumLength(100);            
        }
    }
}
