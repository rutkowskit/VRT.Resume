using FluentValidation;

namespace VRT.Resume.Application.Persons.Commands.UpsertPersonExperience
{
    public sealed class UpsertPersonExperienceCommandValidator : AbstractValidator<UpsertPersonExperienceCommand>
    {
        public UpsertPersonExperienceCommandValidator()
        {
            RuleFor(v => v.CompanyName).MinimumLength(1).NotEmpty();
            RuleFor(v => v.Position).MinimumLength(1).NotEmpty();               
            RuleFor(v => v.Location).MinimumLength(1).NotEmpty();
            RuleFor(v => v.FromDate).NotEmpty();

            RuleFor(v => v.ToDate)
                .Must((v, m) => m == null || v.FromDate.Date <= m.Value)
                .WithMessage(Errors.ChronologyError);
                
        }
    }
}
