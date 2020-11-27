using FluentValidation;

namespace VRT.Resume.Application.Persons.Commands.UpsertPersonEducation
{
    public sealed class UpsertPersonEducationCommandValidator : AbstractValidator<UpsertPersonEducationCommand>
    {
        public UpsertPersonEducationCommandValidator()
        {

            RuleFor(v => v.Degree)
                .MinimumLength(1)
                .NotEmpty();

            RuleFor(v => v.SchoolName)
                .MinimumLength(2)
                .NotEmpty();

            RuleFor(v => v.FromDate)                
                .NotEmpty();

            RuleFor(v => v.ToDate)
                .NotEmpty()
                .GreaterThanOrEqualTo(v => v.ToDate);
        }
    }
}
