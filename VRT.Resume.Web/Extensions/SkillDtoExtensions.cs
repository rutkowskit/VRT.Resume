using System.Collections.Generic;
using System.Linq;
using VRT.Resume.Application.Resumes.Queries.GetResume;
using VRT.Resume.Domain.Common;

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
            => data.GetSkills(SkillTypes.HumanLanguage);

        public static IEnumerable<SkillDto> GetTechnicalSkills(this IEnumerable<SkillDto> data, bool onlyVisible=true)
            => data.GetSkills(SkillTypes.Technical)
                   .Where(s => !onlyVisible || !s.IsHidden)
                   .OrderByDescending(o=>o.Position);

        public static IEnumerable<SkillDto> GetSoftSkills(this IEnumerable<SkillDto> data)
            => data.GetSkills(SkillTypes.Soft);
                    

        public static IEnumerable<SkillDto> GetSkills(this IEnumerable<SkillDto> data,
            SkillTypes type)
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