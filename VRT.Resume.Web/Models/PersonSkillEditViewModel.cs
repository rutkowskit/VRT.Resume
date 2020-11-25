using System.ComponentModel.DataAnnotations;
using VRT.Resume.Application;

namespace VRT.Resume.Web.Models
{
    public class PersonSkillEditViewModel
    {        
        public int? SkillId { get; set; }
        /// <summary>
        /// Skill type
        /// </summary>        
        [Display(Name = "Type", ResourceType = typeof(Resources.LabelResource))]
        public SkillTypes SkillType { get; set; }
        /// <summary>
        /// Skill name
        /// </summary>

        [Required]
        [Display(Name = "Name", ResourceType = typeof(Resources.LabelResource))]
        public string SkillName { get; set; }

        /// <summary>
        /// Skill level description
        /// </summary>
        [Required]
        [Display(Name = "Level", ResourceType = typeof(Resources.LabelResource))]
        public string SkillLevel { get; set; }
    }
}