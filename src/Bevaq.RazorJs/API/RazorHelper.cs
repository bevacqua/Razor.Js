using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Bevaq.RazorJs
{
    /// <summary>
    /// Razor helper, intended to be using alongside HtmlHelper, injected into base View, and used as @Razor.Template(name)
    /// </summary>
    public sealed class RazorHelper
    {
        private const string PARTIAL_NOT_FOUND = "Partial View {0} not found";
        private const string VIEW_TYPE_UNSUPPORTED = "View Type {0} is unsupported";

        private readonly RazorJsEngine engine = new RazorJsEngine();
        private readonly HtmlHelper helper;

        /// <summary>
        /// Constructs an instance of the RazorHelper.
        /// </summary>
        /// <param name="helper">The HtmlHelper to use as a reference to the View.</param>
        public RazorHelper(HtmlHelper helper)
        {
            if (helper == null)
            {
                throw new ArgumentNullException("helper");
            }
            this.helper = helper;
        }

        /// <summary>
        /// Instances a client-side razor templating function.
        /// </summary>
        /// <param name="templateName">The template name.</param>
        /// <returns>The JavaScript function that can render a model with the provided template.</returns>
        public IHtmlString Template(string templateName)
        {
            StringWriter buffer = new StringWriter();

            using (TextReader reader = GetPartialViewStream(templateName))
            {
                string template = reader.ReadToEnd();
                string raw = engine.Parse(template);
                return new HtmlString(raw);
            }
        }

        /// <summary>
        /// Gets a stream reader to the Partial View.
        /// </summary>
        /// <param name="templateName">The template name.</param>
        /// <returns>A text reader to the physical path of the View.</returns>
        private TextReader GetPartialViewStream(string templateName)
        {
            if (templateName == null)
            {
                throw new ArgumentNullException("templateName");
            }
            ControllerContext controllerContext = helper.ViewContext;
            ViewEngineResult view = ViewEngines.Engines.FindPartialView(controllerContext, templateName);

            if (view == null || view.View == null)
            {
                throw new RazorJsException(PARTIAL_NOT_FOUND.FormatWith(templateName));
            }
            RazorView razorView = view.View as RazorView;

            if (razorView == null)
            {
                throw new RazorJsException(VIEW_TYPE_UNSUPPORTED.FormatWith(view.View.GetType()));
            }
            string location = controllerContext.HttpContext.Server.MapPath(razorView.ViewPath);

            return new StreamReader(location);
        }
    }
}