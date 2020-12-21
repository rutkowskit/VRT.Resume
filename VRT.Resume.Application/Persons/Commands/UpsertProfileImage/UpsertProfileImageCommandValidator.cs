using FluentValidation;

namespace VRT.Resume.Application.Persons.Commands.UpsertProfileImage
{
    public sealed class UpsertProfileImageCommandValidator : AbstractValidator<UpsertProfileImageCommand>
    {
        public UpsertProfileImageCommandValidator()
        {
            RuleFor(v => v.ImageData)
                .NotEmpty()
                .ChildRules(c => c.RuleFor(p => p.Length > 0));            
        }
    }
}
