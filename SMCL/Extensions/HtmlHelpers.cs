using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SMCL.Extensions
{
    public static class HtmlHelpers
    {
        public static MvcHtmlString ActionImage(this HtmlHelper html, string action, object routeValues, string imagePath, string alt)
        {
            var url = new UrlHelper(html.ViewContext.RequestContext);

            // build the <img> tag
            var imgBuilder = new TagBuilder("img");
            imgBuilder.MergeAttribute("src", url.Content(imagePath));
            imgBuilder.MergeAttribute("alt", alt);
            string imgHtml = imgBuilder.ToString(TagRenderMode.SelfClosing);

            // build the <a> tag
            var anchorBuilder = new TagBuilder("a");
            anchorBuilder.MergeAttribute("href", url.Action(action, routeValues));
            anchorBuilder.InnerHtml = imgHtml; // include the <img> tag inside
            string anchorHtml = anchorBuilder.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(anchorHtml);
        }

        public static HtmlString Image(this HtmlHelper helper, string id, string url, string alternateText, object htmlAttributes)
        {
            // Instantiate a UrlHelper 
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);

            // Create tag builder
            var builder = new TagBuilder("img");

            // Create valid id
            builder.GenerateId(id);

            // Add attributes
            builder.MergeAttribute("src", urlHelper.Content(url));
            builder.MergeAttribute("alt", alternateText);
            builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            // Render tag
            var ret = new MvcHtmlString(builder.ToString(TagRenderMode.SelfClosing));
            return ret;//.ToHtmlString();
        }
    }
}