using iTextSharp.tool.xml.pipeline.html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExhibitsPortal.Utils.HtmlToPdf
{
    public class LinkProvider : ILinkProvider
    {
        // rfc1738 - file URI scheme section 3.10
        public const char SEPARATOR = '/';
        public string BaseUrl { get; private set; }

        public LinkProvider(UriHelper uriHelper)
        {
            var uri = uriHelper.BaseUri;
            /* simplified implementation that only takes into account:
             * Uri.UriSchemeFile || Uri.UriSchemeHttp || Uri.UriSchemeHttps
             */
            BaseUrl = uri.Scheme == Uri.UriSchemeFile
                // need trailing separator or file paths break
                ? uri.AbsoluteUri.TrimEnd(SEPARATOR) + SEPARATOR
                // assumes Uri.UriSchemeHttp || Uri.UriSchemeHttps
                : BaseUrl = uri.AbsoluteUri;
        }

        public string GetLinkRoot()
        {
            return BaseUrl;
        }
    }
}