using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Humanizer;
using System.Net;

namespace DaxnetBlog.Web
{
    public static class MethodExtensions
    {
        private static readonly Regex Tags = new Regex(@"<[^>]+?>", RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex NotOkCharacter = new Regex(@"[^\w;&#@.:/\\?=|%!() -]", RegexOptions.Compiled);

        public static string ToHtmlString(this TagBuilder tb)
        {
            using (var writer = new StringWriter())
            {
                tb.WriteTo(writer, HtmlEncoder.Default);
                return writer.ToString();
            }
        }

        public static string Summary(this string fullText, int truncateSummaryLength = 50)
        {
            var plainText = RemoveHtmlTags(fullText);
            return plainText.Truncate(truncateSummaryLength, Truncator.FixedNumberOfWords);
        }

        private static string RemoveHtmlTags(string html)
        {
            html = WebUtility.UrlDecode(html);
            html = WebUtility.HtmlDecode(html);

            html = RemoveTag(html, "<!--", "-->");
            html = RemoveTag(html, "<script", "</script>");
            html = RemoveTag(html, "<style", "</style>");

            //replace matches of these regexes with space
            html = Tags.Replace(html, " ");
            html = NotOkCharacter.Replace(html, " ");
            html = SingleSpacedTrim(html);

            return html;
        }

        private static String SingleSpacedTrim(String inString)
        {
            var sb = new StringBuilder();
            var inBlanks = false;
            foreach (var c in inString)
            {
                switch (c)
                {
                    case '\r':
                    case '\n':
                    case '\t':
                    case ' ':
                        if (!inBlanks)
                        {
                            inBlanks = true;
                            sb.Append(' ');
                        }
                        continue;
                    default:
                        inBlanks = false;
                        sb.Append(c);
                        break;
                }
            }
            return sb.ToString().Trim();
        }

        private static String RemoveTag(string html, string startTag, string endTag)
        {
            bool bAgain;
            do
            {
                bAgain = false;
                var startTagPos = html.IndexOf(startTag, 0, StringComparison.CurrentCultureIgnoreCase);
                if (startTagPos < 0) continue;
                var endTagPos = html.IndexOf(endTag, startTagPos + 1, StringComparison.CurrentCultureIgnoreCase);
                if (endTagPos <= startTagPos) continue;
                html = html.Remove(startTagPos, endTagPos - startTagPos + endTag.Length);
                bAgain = true;
            } while (bAgain);
            return html;
        }
    }
}
