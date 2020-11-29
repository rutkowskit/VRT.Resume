namespace VRT.Resume.Web.Models
{
    public sealed class EditDeleteToolbarData
    {
        public EditDeleteToolbarData(string controller, int entityId)
            : this(controller, entityId,"")
        {
            Controller = controller;
            EntityId = entityId;
        }
        public EditDeleteToolbarData(string controller, int entityId, string label)
        {
            Controller = controller;
            EntityId = entityId;
            Label = label;
        }


        public string Controller { get; }        
        public int EntityId { get; }
        public string Label { get; }
    }
}