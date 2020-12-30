using System.ComponentModel.DataAnnotations;

namespace VRT.Resume.Mvc.Models
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
                
        [Display(Name = LabelNames.Icon, ResourceType = typeof(Resources.LabelResource))]        
        public string Icon { get; internal set; }        
        public string Url { get; internal set; }
    }
}
