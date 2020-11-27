using FluentValidation;

namespace VRT.Resume.Application.Persons.Commands.DeletePersonEducation
{
    public sealed class DeletePersonEducationCommandValidator : AbstractValidator<DeletePersonEducationCommand>
    {
        public DeletePersonEducationCommandValidator()
        {
            RuleFor(v => v.EducationId).GreaterThan(0);            
        }
    }
}
