using Microsoft.AspNetCore.Html;
using VRT.Resume.Application;
using VRT.Resume.Application.Resumes.Queries.GetResume;

namespace VRT.Resume.Mvc
{
    public static class ContactItemDtoExtensions
    {
        public static IHtmlContent GetIconHtmlString(this ContactItemDto contact)
        {
            return contact?.Icon == null
                ? null
                : new HtmlString(contact.Icon);                        
        }

        public static IHtmlContent GetValueHtmlString(this ContactItemDto contact)
        {
            if (contact == null) return null;
            var type = !string.IsNullOrWhiteSpace(contact.Url)
                ? ContactItemTypes.Link
                : ContactItemTypes.PlainText;
            switch (type)
            {
                case ContactItemTypes.PlainText:
                    return new HtmlString($"<span>{contact.Value}</span>");
                case ContactItemTypes.Link:
                    return new HtmlString($"<a href=\"{contact.Url}\" target=\"_blank\">{contact.Value}</a>");
                default:
                    return new HtmlString($"Unknown contact type: {type.ToString()}");             
            }            
        }
    }
}
