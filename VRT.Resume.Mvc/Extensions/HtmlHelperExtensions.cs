using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Html;
using System.Threading.Tasks;
using VRT.Resume.Resources;
using VRT.Resume.Application.Common.Queries.GetSupportedLanguages;

namespace VRT.Resume.Mvc
{
    public static class HtmlHelperExtensions
    {
        public static async Task<IHtmlContent> ProfileTabPane(this IHtmlHelper html,
           string tabName, string currentTabName)
        {
            var render = tabName == currentTabName
                ? await html.PartialAsync(tabName)
                : null;
            var activeClass = render == null ? "" : "active";
            var htmlText = $"<div class=\"tab-pane {activeClass}\" id=\"{tabName}\">{render}</div>";
            return new HtmlString(htmlText);
        }
        public static IHtmlContent FormatLabel(this IHtmlHelper html,
           string resourceKey, params object[] formatValues)
        {
            var value = resourceKey.GetLabelText(formatValues);
            return new HtmlString(value);
        }

        public static IHtmlContent GetLabel(this IHtmlHelper html,
           string resourceKey)
        {
            var value = resourceKey.GetLabelText();
            if (string.IsNullOrWhiteSpace(value))
                value = resourceKey;
            return new HtmlString(value);
        }

        public static IHtmlContent GetMessage(this IHtmlHelper html, string resourceKey)
        {
            var value = resourceKey.GetMessageText();
            if (string.IsNullOrWhiteSpace(value))
                value = resourceKey;
            return new HtmlString(value);
        }

        public static IHtmlContent CulturesDropdown(this IHtmlHelper html,
            IEnumerable<LanguageVM> model, string name)
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