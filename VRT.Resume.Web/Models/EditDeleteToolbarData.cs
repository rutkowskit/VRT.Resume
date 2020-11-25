namespace VRT.Resume.Web.Models
{
    public sealed class EditDeleteToolbarData
    {
        public EditDeleteToolbarData(string controller, int entityId)
        {
            Controller = controller;
            EntityId = entityId;
        }

        public string Controller { get; }
        public int EntityId { get; }
    }
}