using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TelegramBots.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static string Token = string.Concat(Guid.NewGuid().ToString(), DateTime.Now.Ticks);

        public static HtmlString RenderScriptLink(this IHtmlHelper helper, string jsPath)
        {
            if (!string.IsNullOrWhiteSpace(jsPath))
            {
                return new HtmlString($"<script src =\"{jsPath}?v={Token}\"></script>");
            }

            return new HtmlString(string.Empty);
        }

        public static HtmlString RenderStyleLink(this IHtmlHelper helper, string cssPath)
        {
            if (!string.IsNullOrWhiteSpace(cssPath))
            {
                return new HtmlString($"<link href =\"{cssPath}?v={Token}\" rel=\"stylesheet\">");
            }

            return new HtmlString(string.Empty);
        }
    }
}