using System;
using System.ComponentModel.DataAnnotations;

namespace VRT.Resume.Web.Models
{
    public sealed class PersonEducationViewModel 
    {
        public int EducationId { get; internal set; }

        [Required]
        [Display(Name = LabelNames.SchoolName, ResourceType = typeof(Resources.LabelResource))]
        public string SchoolName { get; internal set; }
        
        [Required]
        [Display(Name = LabelNames.Degree, ResourceType = typeof(Resources.LabelResource))]
        public string Degree { get; internal set; }
        [Display(Name = LabelNames.Field, ResourceType = typeof(Resources.LabelResource))]
        public string Field { get; internal set; }

        [Required]
        [Display(Name = LabelNames.FromDate, ResourceType = typeof(Resources.LabelResource))]
        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FromDate { get; internal set; }

        [Required]
        [Display(Name=LabelNames.ToDate, ResourceType = typeof(Resources.LabelResource))]
        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; internal set; }

        [Display(Name = LabelNames.Grade, ResourceType = typeof(Resources.LabelResource))]
        public string Grade { get; internal set; }
        
        [Display(Name = LabelNames.ThesisTitle, ResourceType = typeof(Resources.LabelResource))]
        public string ThesisTitle { get; internal set; }
        
        [Display(Name = LabelNames.Specialization, ResourceType = typeof(Resources.LabelResource))]
        public string Specialization { get; internal set; }
        
    }
}
