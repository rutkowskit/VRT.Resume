using VRT.Resume.Domain.Entities;

namespace VRT.Resume.Application
{
    internal static class SkillHelper
    {
        public static PersonSkill CreateSkill(int skillId = 1,
            int personId = Defaults.PersonId, SkillTypes skillType = SkillTypes.Technical)
        {
            return new PersonSkill()
            {
                SkillId = skillId,
                Level = "High",
                PersonId = personId,
                SkillTypeId = (byte)skillType
            };
        }
    }
}
