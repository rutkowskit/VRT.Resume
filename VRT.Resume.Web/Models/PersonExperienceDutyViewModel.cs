using System.ComponentModel.DataAnnotations;

namespace VRT.Resume.Web.Models
{
    public sealed class PersonExperienceDutyViewModel
    {
        public int DutyId { get; internal set; }
        public int ExperienceId { get; internal set; }
        [Required]
        [Display(Name = LabelNames.Name, ResourceType = typeof(Resources.LabelResource))]
        public string Name { get; internal set; }
    }
}
