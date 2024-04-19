using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;

namespace CCAF.Models
{
    public static class CustomHTMLHelper
    {
        public static MvcHtmlString Image(this HtmlHelper helper, string src, string altText,[Optional] string height, [Optional] string onclickfns)
        {
            var builder = new TagBuilder("img");
            builder.MergeAttribute("src", src);
            builder.MergeAttribute("alt", altText);
            builder.MergeAttribute("height", height);
            builder.MergeAttribute("onclick", onclickfns);
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
        }
    }
}