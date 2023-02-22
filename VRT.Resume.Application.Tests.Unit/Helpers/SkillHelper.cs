using VRT.Resume.Domain.Common;
using VRT.Resume.Domain.Entities;

namespace VRT.Resume.Application;

internal static class SkillHelper
{
    public static PersonSkill CreateSkill(this Person person, SkillTypes skillType = SkillTypes.Technical)
    {
        return new PersonSkill()
        {                
            Level = "High",
            PersonId = person.PersonId,
            SkillTypeId = (byte)skillType,
            Name = "Some nice skill"
        };
    }
}
