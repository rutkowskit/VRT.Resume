using Autofac;
using System.Threading.Tasks;
using VRT.Resume.Domain.Entities;

namespace VRT.Resume.Application.Persons.Commands.DeletePersonEducation
{
    public class DeletePersonEducationCommandTests 
        : DeleteCommandTestBase<DeletePersonEducationCommand,PersonEducation>
    {
        protected override DeletePersonEducationCommand CreateSut(int educationId)
        {
            return new DeletePersonEducationCommand(educationId);
        }

        protected override Task SeedEntity(ILifetimeScope scope)
        {
            return scope.SeedEducation();
        }
    }
}