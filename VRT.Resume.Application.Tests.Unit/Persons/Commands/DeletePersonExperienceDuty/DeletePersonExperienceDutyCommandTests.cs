using VRT.Resume.Domain.Entities;
using System.Threading.Tasks;
using Autofac;

namespace VRT.Resume.Application.Persons.Commands.DeletePersonExperienceDuty
{
    public class DeletePersonExperienceDutyCommandTests
        : DeleteCommandTestBase<DeletePersonExperienceDutyCommand, PersonExperienceDuty>
    {
        protected override DeletePersonExperienceDutyCommand CreateSut(int entityId)
        {
            return new DeletePersonExperienceDutyCommand(entityId);
        }

        protected override Task SeedEntity(ILifetimeScope scope)
        {
            return scope.SeedExperience();
        }
    }
}