using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq.Expressions;

namespace VRT.Resume.Mvc
{
    public static partial class HtmlHelperExtensions
    {
        public static IHtmlContent FullEditorFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TResult>> expression)
        {
            var result = new HtmlContentBuilder()
                .AppendHtml(htmlHelper.LabelFor(expression, htmlAttributes: new { @class = "control-label" }))
                .AppendHtml(htmlHelper.EditorFor(expression, new { htmlAttributes = new { @class = "form-control" } }))
                .AppendHtml(htmlHelper.ValidationMessageFor(expression, "", new { @class = "text-danger" }));
            
            return result;
        }
        public static IHtmlContent FormGroupWithFullEditorFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TResult>> expression, string groupClasses)
        {
            var outerDiv = new TagBuilder("div");
            outerDiv.AddCssClass("form-group");
            if(!string.IsNullOrWhiteSpace(groupClasses))
            {
                outerDiv.AddCssClass(groupClasses);
            }
            outerDiv.InnerHtml.AppendHtml(htmlHelper.FullEditorFor(expression));
            return outerDiv;
        }
    }
}