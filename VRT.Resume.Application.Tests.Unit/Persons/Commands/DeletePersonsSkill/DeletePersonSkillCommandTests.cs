using VRT.Resume.Domain.Entities;
using System.Threading.Tasks;
using Autofac;

namespace VRT.Resume.Application.Persons.Commands.DeletePersonsSkill
{
    public class DeletePersonSkillCommandTests
        : DeleteCommandTestBase<DeletePersonSkillCommand, PersonSkill>
    {
        protected override DeletePersonSkillCommand CreateSut(int entityId)
        {
            return new DeletePersonSkillCommand(entityId);
        }

        protected override Task SeedEntity(ILifetimeScope scope)
        {
            return scope.SeedExperience();
        }
    }
}