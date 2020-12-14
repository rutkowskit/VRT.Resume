using VRT.Resume.Domain.Entities;
using System.Threading.Tasks;
using Autofac;

namespace VRT.Resume.Application.Persons.Commands.DeletePersonExperience.Tests
{
    public class DeletePersonExperienceCommandTests
        : DeleteCommandTestBase<DeletePersonExperienceCommand, PersonExperience>
    {
        protected override DeletePersonExperienceCommand CreateSut(int entityId)
        {
            return new DeletePersonExperienceCommand(entityId);
        }

        protected override Task SeedEntity(ILifetimeScope scope)
        {
            return scope.SeedExperience();
        }
    }
}