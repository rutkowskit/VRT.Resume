using System.Web;
using VRT.Resume.Application;
using VRT.Resume.Application.Resumes.Queries.GetResume;

namespace VRT.Resume.Web
{
    public static class ContactItemDtoExtensions
    {
        public static IHtmlString GetIconHtmlString(this ContactItemDto contact)
        {
            return contact?.Icon == null
                ? null
                : new HtmlString(contact.Icon);                        
        }

        public static IHtmlString GetValueHtmlString(this ContactItemDto contact)
        {
            if (contact == null) return null;
            switch(contact.Type)
            {
                case ContactItemTypes.PlainText:
                    return new HtmlString($"<span>{contact.Value}</span>");
                case ContactItemTypes.Link:
                    return new HtmlString($"<a href=\"{contact.Url}\" target=\"_blank\">{contact.Value}</a>");
                default:
                    return new HtmlString($"Unknown contact type: {contact.Type.ToString()}");             
            }            
        }
    }
}
