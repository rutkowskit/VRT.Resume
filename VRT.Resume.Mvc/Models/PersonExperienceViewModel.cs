using System;
using System.ComponentModel.DataAnnotations;

namespace VRT.Resume.Mvc.Models
{
    public sealed class PersonExperienceViewModel
    {
        public int ExperienceId { get; internal set; }

        [Required]
        [Display(Name = LabelNames.Position, ResourceType = typeof(Resources.LabelResource))]
        public string Position { get; internal set; }
        
        [Required]
        [Display(Name = LabelNames.CompanyName, ResourceType = typeof(Resources.LabelResource))]
        public string CompanyName { get; internal set; }

        [Display(Name = LabelNames.Location, ResourceType = typeof(Resources.LabelResource))]
        public string Location { get; internal set; }

        [Required]
        [Display(Name = LabelNames.FromDate, ResourceType = typeof(Resources.LabelResource))]
        [DataType(DataType.Date)]        
        public DateTime FromDate { get; internal set; }

        [Display(Name=LabelNames.ToDate, ResourceType = typeof(Resources.LabelResource))]
        [DataType(DataType.Date)]        
        public DateTime? ToDate { get; internal set; }        
    }
}
