using System.Web;
using System.Web.Mvc.Html;

namespace VRT.Resume.Web
{
    public static class HtmlHelperExtensions
    {

        public static IHtmlString ProfileTabPane(this System.Web.Mvc.HtmlHelper html,
           string tabName, string currentTabName)
        {
            var render = tabName == currentTabName
                ? html.Partial(tabName).ToHtmlString()
                : null;
            var activeClass = render == null ? "" : "active";
            var htmlText = $"<div class=\"tab-pane {activeClass}\" id=\"{tabName}\">{render}</div>";            
            return new HtmlString(htmlText);
        }

        public static IHtmlString GetLabel(this System.Web.Mvc.HtmlHelper html,
           string resourceKey, string defaultValue=null)
        {
            var value = Resources.LabelResource.ResourceManager.GetString(resourceKey) 
                ?? defaultValue;
            if (string.IsNullOrWhiteSpace(value))
                value = resourceKey;  
            
            return new HtmlString(value);
        }
    }
}