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

        internal static async Task SeedExperience(this ILifetimeScope scope,
            int experienceId = 1,
            int personId = Defaults.PersonId)
        {
            var db = scope.Resolve<AppDbContext>();

            var toAdd = new PersonExperience()
            {
                ExperienceId = experienceId,                
                PersonId = personId,
                CompanyName = "CompanyName",
                FromDate = Defaults.Today.AddDays(-10),
                ToDate = Defaults.Today.AddDays(-1),
                Location = "Location",
                ModifyDate = Defaults.Today,
                Position = "Position",
                PersonExperienceDuty = new List<PersonExperienceDuty>()
                {
                    new PersonExperienceDuty()
                    {
                        DutyId = 1,
                        Name = "DutyName",                        
                        PersonExperienceDutySkill = new List<PersonExperienceDutySkill>()
                        {
                            new PersonExperienceDutySkill()
                            {
                                Id = 1,
                                Skill = new PersonSkill()
                                {
                                    SkillId = 1,
                                    Level = "High",
                                    PersonId = personId,
                                    SkillTypeId = (int)SkillTypes.Technical                                    
                                }                                
                            }
                        }
                    }
                }
            };
            db.PersonExperience.Add(toAdd);
            await db.SaveChangesAsync();
        }
    }
}
