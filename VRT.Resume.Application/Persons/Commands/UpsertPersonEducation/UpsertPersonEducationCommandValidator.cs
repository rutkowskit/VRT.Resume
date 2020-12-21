using FluentValidation;

namespace VRT.Resume.Application.Persons.Commands.UpsertPersonEducation
{
    public sealed class UpsertPersonEducationCommandValidator : AbstractValidator<UpsertPersonEducationCommand>
    {
        public UpsertPersonEducationCommandValidator()
        {

            RuleFor(v => v.Degree)
                .NotEmpty()
                .MinimumLength(1);

            RuleFor(v => v.SchoolName)
                .NotEmpty()
                .MinimumLength(2);

            RuleFor(v => v.FromDate)                
                .NotEmpty();

            RuleFor(v => v.ToDate)
                .NotEmpty()
                .GreaterThanOrEqualTo(v => v.ToDate);
        }
    }
}
