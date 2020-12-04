using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using VRT.Resume.Web.Models;

namespace VRT.Resume.Web
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlString ProfileTabPane(this HtmlHelper html,
           string tabName, string currentTabName)
        {
            var render = tabName == currentTabName
                ? html.Partial(tabName).ToHtmlString()
                : null;
            var activeClass = render == null ? "" : "active";
            var htmlText = $"<div class=\"tab-pane {activeClass}\" id=\"{tabName}\">{render}</div>";
            return new HtmlString(htmlText);
        }

        public static IHtmlString GetLabel(this HtmlHelper html,
           string resourceKey, string defaultValue = null)
        {
            var value = Resources.LabelResource.ResourceManager.GetString(resourceKey)
                ?? defaultValue;
            if (string.IsNullOrWhiteSpace(value))
                value = resourceKey;

            return new HtmlString(value);
        }

        public static IHtmlString GetMessage(this HtmlHelper html,
           string resourceKey, string defaultValue = null)
        {
            var value = Resources.MessageResource.ResourceManager.GetString(resourceKey)
                ?? defaultValue;
            if (string.IsNullOrWhiteSpace(value))
                value = resourceKey;

            return new HtmlString(value);
        }

        public static IHtmlString CulturesDropdown(this HtmlHelper html,
            IEnumerable<CultureSettingsViewModel> model, string name)
        {            
            var cultures = model.Select(c => new SelectListItem()
            {
                Value = c.Key,
                Text = c.Caption,
                Selected = (string)html.ViewContext.TempData[TempDataKeys.CultureKey] == c.Key
            }).ToArray();
            return html.DropDownList(name, cultures, new { @class = "form-control form-control-lg col-md-2" });
        }
    }
}