using Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application
{
    internal static class LifetimeScopeExtensions
    {
        internal static async Task SeedContact(this ILifetimeScope scope, 
            int contactId=1,
            int personId = Defaults.PersonId)
        {
            var db = scope.Resolve<AppDbContext>();

            var toAdd = new PersonContact()
            {
                ContactId = contactId,
                PersonId = personId ,
                Name = "Default",
                Value = "Default",
                Icon = "<svg></svg>",
                Url = "http://test.me",
                ModifiedDate = new DateTime(2020, 2, 3)
            };
            db.PersonContact.Add(toAdd);
            await db.SaveChangesAsync();
        }

        internal static async Task SeedEducation(this ILifetimeScope scope,
            int educationId = 1,
            int personId = Defaults.PersonId)
        {
            var db = scope.Resolve<AppDbContext>();

            var toAdd = new PersonEducation()
            {
                EducationId = educationId,
                PersonId = personId,                
                ModifiedDate = new DateTime(2020, 2, 3)
            };
            db.PersonEducation.Add(toAdd);
            await db.SaveChangesAsync();
        }
    }
}
