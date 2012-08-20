using System.Web;
using System.Web.Mvc;

namespace Bevaq.RazorJs
{
    /// <summary>
    /// RazorJs extensions to HtmlHelper for easier setup than using RazorHelper directly.
    /// </summary>
    public static class RazorJsExtensions
    {
        /// <summary>
        /// Instances a client-side razor templating function.
        /// </summary>
        /// <param name="html">The HtmlHelper.</param>
        /// <param name="templateName">The template name.</param>
        /// <returns>The JavaScript function that can render a model with the provided template.</returns>
        public static IHtmlString RazorTemplate(this HtmlHelper html, string templateName)
        {
            RazorHelper helper = new RazorHelper(html);
            return helper.Template(templateName);
        }
    }
}
