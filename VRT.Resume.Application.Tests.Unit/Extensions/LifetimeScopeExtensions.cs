using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using VRT.Resume.Domain.Common;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application;

internal static class LifetimeScopeExtensions
{
    internal static async Task<PersonContact> SeedContact(this AppDbContext db)
    {
        var person = db.GetFirstPerson();
        var toAdd = new PersonContact()
        {            
            PersonId = person.PersonId,
            Name = "Default",
            Value = "Default",
            Icon = "<svg></svg>",
            Url = "http://test.me",
            ModifiedDate = new DateTime(2020, 2, 3)
        };
        db.PersonContact.Add(toAdd);
        await db.SaveChangesAsync();
        return toAdd;
    }
    internal static async Task<PersonSkill> SeedSkill(this AppDbContext db,                
        string name = "Skill",
        string level = "10",
        SkillTypes type = SkillTypes.Technical)
    {
        var person = db.GetFirstPerson();
        var toAdd = new PersonSkill()
        {            
            SkillTypeId = (byte)type,
            Level = level,
            Name = name,
            PersonId = person.PersonId
        };
        db.PersonSkill.Add(toAdd);
        await db.SaveChangesAsync();
        return toAdd;
    }

    internal static async Task<PersonEducation> SeedEducation(this AppDbContext db)
    {
        var person = db.GetFirstPerson();
        var toAdd = new PersonEducation()
        {            
            PersonId = person.PersonId,
            ModifiedDate = Defaults.Today,
            School = new School()
            {                
                Name = "School",
                ModifiedDate = Defaults.Today
            }
        };
        db.PersonEducation.Add(toAdd);
        await db.SaveChangesAsync();
        return toAdd;
    }

    internal static async Task<School> SeedSchool(this AppDbContext db,
        string schoolName = "School")
    {
        var toAdd = new School()
        {
            Name = schoolName,
            ModifiedDate = Defaults.Today
        };
        db.School.Add(toAdd);
        await db.SaveChangesAsync();
        return toAdd;
    }

    internal static async Task<Degree> SeedDegree(this AppDbContext db,        
        string degreeName = "Master")
    {
        var toAdd = new Degree()
        {            
            Name = degreeName
        };
        db.Degree.Add(toAdd);
        await db.SaveChangesAsync();
        return toAdd;
    }

    internal static async Task<EducationField> SeedEducationField(this AppDbContext db,        
        string educationFieldName = "Science")
    {
        var toAdd = new EducationField()
        {
            Name = educationFieldName
        };
        db.EducationField.Add(toAdd);
        await db.SaveChangesAsync();
        return toAdd;
    }

    internal static async Task<PersonExperience> SeedExperience(this AppDbContext db,
        bool seedDuty = true,
        bool seedSkill = true)
    {
        var person = db.GetFirstPerson();
        var toAdd = new PersonExperience()
        {
            //ExperienceId = experienceId,
            PersonId = person.PersonId,
            CompanyName = "CompanyName",
            FromDate = Defaults.Today.AddDays(-10),
            ToDate = Defaults.Today.AddDays(-1),
            Location = "Location",
            ModifyDate = Defaults.Today,
            Position = "Position",
        };
        if (seedDuty)
        {
            var duty = new PersonExperienceDuty()
            {                
                Name = "DutyName"
            };
            if (seedSkill)
            {
                new List<PersonExperienceDutySkill>()
                {
                    new PersonExperienceDutySkill()
                    {                        
                        Skill = person.CreateSkill()
                    }
                }.ForEach(duty.PersonExperienceDutySkill.Add);
            }
            new List<PersonExperienceDuty>()
            {
              duty
            }.ForEach(toAdd.PersonExperienceDuty.Add);
        }
        db.PersonExperience.Add(toAdd);
        await db.SaveChangesAsync();
        return toAdd;
    }

    internal static async Task<PersonResume> SeedPersonResume(this AppDbContext db)
    {
        var person = db.GetFirstPerson();
        var toAdd = new PersonResume()
        {
            PersonId = person.PersonId,
            Description = "Description",
            Permission = "Permission granted",
            ModifiedDate = Defaults.Today,
            Summary = "Person experience summary",
            ShowProfilePhoto = true,
            Position = "Computer scientist",
        };
        new List<ResumePersonSkill>()
        {
            new ResumePersonSkill()
            {
                Skill = person.CreateSkill(),
                Position = 1
            }
        }.ForEach(toAdd.ResumePersonSkill.Add);
        db.PersonResume.Add(toAdd);
        await db.SaveChangesAsync();
        return toAdd;
    }

    internal static async Task<PersonImage> SeedImage(this AppDbContext db)
    {
        var person = db.GetFirstPerson();
        var toAdd = new PersonImage()
        {            
            ImageType = "some/image",
            ImageData = new byte[] { 0x66, 0x55 },
            PersonId = person.PersonId            
        };
        db.PersonImage.Add(toAdd);
        await db.SaveChangesAsync();
        return toAdd;
    }

    private static Person GetFirstPerson(this AppDbContext db)
    {
        var person = db
            .UserPerson
            .Where(up => up.UserId == Defaults.UserId)
            .Select(up => up.Person)
            .AsNoTracking()
            .First();
        return person;
    }
}
