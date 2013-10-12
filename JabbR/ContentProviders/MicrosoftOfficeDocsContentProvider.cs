using System;
using System.Collections.Generic;
using System.Linq;

using JabbR.ContentProviders.Core;

namespace JabbR.ContentProviders
{
    public class MicrosoftOfficeDocsContentProvider : EmbedContentProvider
    {
        private const string EmbedCode =
            @"<iframe src=""https://view.officeapps.live.com/op/embed.aspx?src={0}"" width=""100%;"" height=""400px"" frameborder=""0"">";

        private static readonly List<string> SupportedExtensions = new List<string>
            {
                "xlsx",
                "xls",
                "docx",
                "doc",
                "pptx",
                "ppt",
            };

        public override IEnumerable<string> Domains
        {
            get
            {
                yield return Uri.UriSchemeHttp;
                yield return Uri.UriSchemeHttps;
            }
        }

        public override string MediaFormatString
        {
            get { return EmbedCode; }
        }

        public override bool IsValidContent(Uri uri)
        {
            return base.IsValidContent(uri) && SupportedExtensions.Any(str => uri.AbsolutePath.EndsWith(str));
        }

        protected override IList<string> ExtractParameters(Uri responseUri)
        {
            return new List<string>
                {
                    responseUri.AbsoluteUri
                };
        }
    }
}