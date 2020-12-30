using FluentValidation;

namespace VRT.Resume.Application.Common.Commands.SetUserLanguage
{
    public sealed class SetUserCultureCommandValidator : AbstractValidator<SetUserCultureCommand>
    {
        public SetUserCultureCommandValidator()
        {
            RuleFor(v => v.NewCulture).MinimumLength(2);
        }
    }
}
