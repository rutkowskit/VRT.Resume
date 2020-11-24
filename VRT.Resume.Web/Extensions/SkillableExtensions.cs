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
            Func<VRT.Resume.Application.Resumes.Queries.GetResume.SkillDto,string> formatter)
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
        private static string FormatSkill(VRT.Resume.Application.Resumes.Queries.GetResume.SkillDto skill)
            => $"<span class=\"{skill.GetCssClass("")}\">{skill.Name}</span>";                        

        //private static string FormatSkillChip(VRT.Resume.Application.Resumes.Queries.GetResume.SkillDto skill)
        //{
        //    return $"<span class=\"{skill.GetCssClass("chip")}\">{skill.Name}</span>";            
        //}
            
    }
}