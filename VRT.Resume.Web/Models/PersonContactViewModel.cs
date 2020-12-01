using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace VRT.Resume.Web.Models
{    
    public sealed class PersonContactViewModel
    {
        public int ContactId { get; internal set; }
        [Required]
        [Display(Name = LabelNames.Name, ResourceType = typeof(Resources.LabelResource))]
        public string Name { get; internal set; }
        [Required]
        [Display(Name = LabelNames.Value, ResourceType = typeof(Resources.LabelResource))]
        public string Value { get; internal set; }

        [AllowHtml]        
        [Display(Name = LabelNames.Icon, ResourceType = typeof(Resources.LabelResource))]        
        public string Icon { get; internal set; }        
        public string Url { get; internal set; }
    }
}
