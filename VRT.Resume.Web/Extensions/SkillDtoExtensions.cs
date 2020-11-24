using System.Collections.Generic;
using VRT.Resume.Application.Resumes.Queries.GetResume;

namespace VRT.Resume.Web
{
    public static class SkillDtoExtensions
    {
        public static string GetCssClass(this SkillDto skill,
            string className, string relevantCssClass = "font-weight-bolder")
        {
            if (skill == null)
                return string.Empty;
            return skill.IsRelevant
                ? $"{className} {relevantCssClass}"
                : className;
        }

        public static IEnumerable<SkillDto> GetLanguageSkills(this IEnumerable<SkillDto> data)
            => data.GetSkills(Application.SkillTypes.HumanLanguage);

        public static IEnumerable<SkillDto> GetTechnicalSkills(this IEnumerable<SkillDto> data)
            => data.GetSkills(Application.SkillTypes.Technical);

        public static IEnumerable<SkillDto> GetSoftSkills(this IEnumerable<SkillDto> data)
            => data.GetSkills(Application.SkillTypes.Soft);

        public static IEnumerable<SkillDto> GetSkills(this IEnumerable<SkillDto> data,
            Application.SkillTypes type)
        {
            if (data == null) yield break;
            foreach (var d in data)
            {
                if (d.Type == type)
                    yield return d;
            }
        }
    }
}