using FluentValidation;

namespace VRT.Resume.Application.Persons.Commands.UpsertPersonContact
{
    public sealed class UpsertPersonContactCommandValidator : AbstractValidator<UpsertPersonContactCommand>
    {
        public UpsertPersonContactCommandValidator()
        {
            RuleFor(v => v.Name).MinimumLength(1).NotEmpty();
            RuleFor(v => v.Value).MinimumLength(1).NotEmpty();
            RuleFor(v => v.Icon)
                .MaximumLength(5000)
                .Must(IsAnImageOrEmpty)
                .WithMessage(Errors.UnsupportedImage);
        }
        private static bool IsAnImageOrEmpty(string iconText)
        {               
            return string.IsNullOrWhiteSpace(iconText)
                || iconText.ToSafeImage() != null;
                
        }
    } 
}
