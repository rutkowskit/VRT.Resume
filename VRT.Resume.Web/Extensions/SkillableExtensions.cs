using System;
using System.Text;
using System.Web;
using VRT.Resume.Application.Resumes.Queries.GetResume;

namespace VRT.Resume.Web
{
    public static class SkillableExtensions
    {
        public static IHtmlString FormatSkills(this ISkillable entity)
            => entity.FormatSkills("(", ")", ", ", FormatSkill);

        public static IHtmlString FormatSkills(this ISkillable entity, 
            string prefix, string postfix, string separator, 
            Func<SkillDto, string> formatter)
        {
            if (entity == null || entity.Skills==null || entity.Skills.Length==0)
                return null;
            
            if (formatter == null)
                formatter = FormatSkill;
            var result = new StringBuilder();
            foreach (var skill in entity.Skills)
            {
                if (skill == null) continue;
                if (result.Length > 0)
                    result.AppendNonEmpty(separator);
                result.AppendNonEmpty(formatter(skill));           
            }
            return new HtmlString($"{prefix}{result}{postfix}");                
        }
        private static string FormatSkill(SkillDto skill)
            => $"<span class=\"{skill.GetCssClass("")}\">{skill.Name}</span>";                                
            
    }
}