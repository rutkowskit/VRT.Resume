using System.ComponentModel.DataAnnotations;

namespace VRT.Resume.Web.Models
{
    public sealed class EditDeleteToolbarData
    {
        public EditDeleteToolbarData(string controller, int entityId)
        {
            Controller = controller;
            EntityId = entityId;
        }

        [Required]
        public string Controller { get; }
        [Required]
        public int EntityId { get; }
    }
}