using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc.Razor;
using System.Web.Razor;
using System.Web.Razor.Parser.SyntaxTree;
using System.Web.Razor.Text;
using log4net;

namespace Bevaq.RazorJs
{
    internal sealed class RazorJsEngine
    {
        private const string EXCEPTION_LOCATION_FORMAT = "Error at line {0}, position {1}:";
        private const string UNHANDLED_EXCEPTION = "Razor engine parsing failed";

        private const string NODE_IGNORED = "Ignoring node: {0}";
        private const string HELPER_UNSUPPORTED = "Ignoring {0} - Helpers not currently supported";

        private readonly ILog log = LogManager.GetLogger(typeof(RazorJsEngine));

        public string Parse(string razorTemplate)
        {
            razorTemplate = StripModelDeclaration(razorTemplate); // model declarations are a hassle. lets get rid of those.

            using (StringWriter writer = new StringWriter())
            using (StringReader reader = new StringReader(razorTemplate))
            {
                Parse(reader, writer);

                string template = writer.GetStringBuilder().ToString();
                return template;
            }
        }

        internal void Parse(TextReader razorTemplate, TextWriter output)
        {
            RazorEngineHost host = new RazorEngineHost(new CSharpRazorCodeLanguage());
            RazorTemplateEngine engine = new RazorTemplateEngine(host);

            ParserResults result = engine.ParseTemplate(razorTemplate);

            if (!result.Success)
            {
                ThrowParserError(result);
            }
            WriteTemplateFunction(result.Document, output);
        }

        internal string StripModelDeclaration(string razorTemplate)
        {
            const string MODEL_DECLARATION_REGEX = ".*@model .*(\r\n|\n)?";
            string stripped = Regex.Replace(razorTemplate, MODEL_DECLARATION_REGEX, "");
            return stripped;
        }

        internal void ThrowParserError(ParserResults result)
        {
            if (result.ParserErrors.Any())
            {
                StringBuilder builder = new StringBuilder();
                foreach (RazorError error in result.ParserErrors)
                {
                    if (result.ParserErrors.IndexOf(error) > 0)
                    {
                        builder.AppendLine();
                        builder.AppendLine();
                    }
                    SourceLocation location = error.Location;
                    builder.AppendFormat(EXCEPTION_LOCATION_FORMAT, location.LineIndex, location.CharacterIndex);
                    builder.AppendLine();
                    builder.AppendLine(error.Message);
                }
                string message = builder.ToString();
                throw new RazorJsException(message);
            }
            throw new RazorJsException(UNHANDLED_EXCEPTION);
        }

        internal void WriteTemplateFunction(Block document, TextWriter output)
        {
            output.WriteLine(@"function (Model) {");
            output.WriteLine("    var buffer = [];");

            ParseSyntaxTreeNode(document, output);

            output.WriteLine("    return buffer.join('');");
            output.Write("}");
        }

        internal void ParseSyntaxTreeNode(SyntaxTreeNode node, TextWriter output)
        {
            if (node == null) return; // sanity.
            if (node is TransitionSpan) return; // ignore the @ symbol.

            // ignore @model and @inherits as part of the transition from static to dynamic typing.
            if (node is ModelSpan) return;
            if (node is InheritsSpan) return;
            if (node is MetaCodeSpan) return;

            // explicitly support these types of node
            if (VisitBlock(node as Block, output)) return;
            if (VisitMarkupSpan(node as MarkupSpan, output)) return;
            if (VisitCodeSpan(node as CodeSpan, output)) return;

            log.DebugFormat(NODE_IGNORED, node); // emit a warning for any node that wasn't handled above.
        }

        internal bool VisitBlock(Block block, TextWriter output)
        {
            if (block == null)
            {
                return false;
            }
            foreach (var child in block.Children)
            {
                ParseSyntaxTreeNode(child, output);
            }
            return true;
        }

        internal bool VisitMarkupSpan(MarkupSpan markup, TextWriter output)
        {
            if (markup == null)
            {
                return false;
            }
            StringBuilder content = new StringBuilder(markup.Content);
            content.Replace("\"", "\\\"");
            content.Replace("'", "\\'");
            content.Replace("\r", "\\r");
            content.Replace("\n", "\\n");

            output.WriteLine("    buffer.push('{0}');", content);
            return true;
        }

        internal bool VisitCodeSpan(CodeSpan code, TextWriter output)
        {
            if (code == null) return false;

            if (code is HelperHeaderSpan)
            {
                log.InfoFormat(HELPER_UNSUPPORTED, code); // TODO: support @Helper methods.
                return true;
            }
            if (code is HelperFooterSpan)
            {
                return true;
            }
            if (code is ImplicitExpressionSpan)
            {
                output.WriteLine("    buffer.push({0});", code.Content);
            }
            else
            {
                StringBuilder codeContent = new StringBuilder(code.Content);
                codeContent.Replace("\r", string.Empty);
                codeContent.Replace("\n", string.Empty);

                string translatedCode = TranslateCodeBlock(codeContent.ToString());
                output.WriteLine(translatedCode);
            }

            return true;
        }

        internal string TranslateCodeBlock(string code)
        {
            const string FOREACH_NODE_REGEX = @"foreach *\( *(?<type>[^ ]*) (?<variable>[^ ]*) in (?<enumerator>[^ )]*)\) *{";
            const string FOR_INITIALIZE = "    var {2} = {1}[z{0}]; ";
            const string FOR_STATEMENT = "for (var z{0} = 0; z{0} < {1}.length; z{0}++) {{";

            Match match = Regex.Match(code, FOREACH_NODE_REGEX);

            if (!match.Success)
            {
                return code;
            }
            GroupCollection groups = match.Groups;
            string enumerator = groups["enumerator"].Value;
            string variable = groups["variable"].Value;
            string identifier = Guid.NewGuid().Stringify().Substring(0, 6);

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(FOR_STATEMENT.FormatWith(identifier, enumerator));
            builder.Append(FOR_INITIALIZE.FormatWith(identifier, enumerator, variable));

            string translation = builder.ToString();
            return translation;
        }
    }
}
