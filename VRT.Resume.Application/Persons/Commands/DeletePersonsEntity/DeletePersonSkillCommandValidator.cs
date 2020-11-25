using FluentValidation;

namespace VRT.Resume.Application.Persons.Commands.DeletePersonsEntity
{
    public sealed class DeletePersonSkillCommandValidator : AbstractValidator<DeletePersonSkillCommand>
    {
        public DeletePersonSkillCommandValidator()
        {
            RuleFor(v => v.SkillId).GreaterThan(0);            
        }
    }
}
