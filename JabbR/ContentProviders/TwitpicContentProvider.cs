using System;
using System.Collections.Generic;
using System.Net;
using JabbR.ContentProviders.Core;
using System.Text.RegularExpressions;

namespace JabbR.ContentProviders
{
    public class TwitPicContentProvider : CollapsibleContentProvider
    {

        static Regex PicIdRegex = new Regex(@"http://(www\.)?twitpic.com/(?<id>\w+)");
        const string FormatString = @"<a href=""http://twitpic.com/{0}""><img src=""http://twitpic.com/show/large/{0}""/></a>";

        protected override ContentProviderResultModel GetCollapsibleContent(HttpWebResponse response)
        {
            var match = PicIdRegex.Match(response.ResponseUri.AbsoluteUri);
            if (match.Success)
            {
                return new ContentProviderResultModel()
                {

                    Content = string.Format(FormatString, match.Groups["id"].Value),
                    Title = response.ResponseUri.AbsoluteUri.ToString()
                };
            }

            return null;
        }

        static Regex ValidUrlRegex = new Regex(@"http://(www\.)?twitpic.com/\w+");

        protected override bool IsValidContent(HttpWebResponse response)
        {
            return ValidUrlRegex.IsMatch(response.ResponseUri.AbsoluteUri);

        }
    }
}