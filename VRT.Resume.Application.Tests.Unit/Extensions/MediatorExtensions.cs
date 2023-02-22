using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application;

internal static class MediatorRequestExtensions
{
    internal static void SeedDbContext(this AppDbContext context)
    {
        context.InitDatabase();
        context.UserPerson.Add(new UserPerson()
        {
            UserId = Defaults.UserId,            
            Person = new Person()
            {                
                FirstName = "Tom",
                LastName = "Tester",
                ModifiedDate = new DateTime(2020, 11, 20)
            }
        });
        context.SaveChanges();
    }
}
