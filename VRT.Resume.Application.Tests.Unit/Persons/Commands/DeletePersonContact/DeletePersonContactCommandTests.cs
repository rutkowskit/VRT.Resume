using System.Threading.Tasks;
using Autofac;
using VRT.Resume.Domain.Entities;

namespace VRT.Resume.Application.Persons.Commands.DeletePersonContact
{
    public class DeletePersonContactCommandTests 
        : DeleteCommandTestBase<DeletePersonContactCommand, PersonContact>
    {        
        protected override Task SeedEntity(ILifetimeScope scope)
            => scope.SeedContact();
        
        protected override DeletePersonContactCommand CreateSut(int contactId)
        {
            return new DeletePersonContactCommand(contactId);
        }
    }
}