using Microsoft.AspNetCore.Html;
using System;
using System.Linq;
using System.Text;
using VRT.Resume.Application.Resumes.Queries.GetResume;

namespace VRT.Resume.Mvc
{
    public static class SkillableExtensions
    {
        public static IHtmlContent FormatSkills(this ISkillable entity)
            => entity.FormatSkills("(", ")", ", ", FormatSkill);

        public static IHtmlContent FormatSkills(this ISkillable entity, 
            string prefix, string postfix, string separator, 
            Func<SkillDto, string> formatter)
        {
            var skills = entity?.Skills
                ?.OrderByDescending(o => o.Position)
                .ThenByDescending(o => o.Level)
                .ToArray();
            if (skills==null || skills.Length==0)
                return null;
            
            if (formatter == null)
                formatter = FormatSkill;
            var result = new StringBuilder();
            foreach (var skill in skills)
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