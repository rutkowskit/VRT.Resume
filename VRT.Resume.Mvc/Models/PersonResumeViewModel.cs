using System;
using System.ComponentModel.DataAnnotations;

namespace VRT.Resume.Mvc.Models
{
    public sealed class PersonResumeViewModel
    {
        public int ResumeId { get; internal set; }

        [Required]
        [Display(Name = LabelNames.Position, ResourceType = typeof(Resources.LabelResource))]
        public string Position { get; internal set; }

        [Required]
        [Display(Name = LabelNames.Summary, ResourceType = typeof(Resources.LabelResource))]
        public string Summary { get; internal set; }

        [Required]
        [Display(Name = LabelNames.ShowProfilePhoto, ResourceType = typeof(Resources.LabelResource))]
        public bool ShowProfilePhoto { get; internal set; }

        [Display(Name = LabelNames.DataProcessingPermission, ResourceType = typeof(Resources.LabelResource))]
        public string DataProcessingPermission { get; internal set; }
        
        [Display(Name = LabelNames.Description, ResourceType = typeof(Resources.LabelResource))]
        public string Description { get; internal set; }
    }
}
