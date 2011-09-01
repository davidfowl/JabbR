using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignalR.Samples.Hubs.Chat;
using HtmlAgilityPack;
using System.Net;
using System.IO;
using System.Text;

// Credit
// http://webcmd.wordpress.com/2011/02/16/c-webservice-for-getting-link-details-like-facebook/

namespace SignalR.Samples.Hubs.Chat.ContentProviders {
    public class WebPageContentProvider : IContentProvider {

        // currently only implementing text/html
        private static readonly HashSet<string> _allowedMimeTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase){
            "text/html"
        };

        private static string ReadPage(HttpWebResponse response) {
            using (var reader = new StreamReader(response.GetResponseStream())) {
                return reader.ReadToEnd();
            }
        }

        private class PageDetails {
            public string Url { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public  string Type { get; set; }
            public  string SiteName { get; set; }
            public  string Email { get; set; }
            public  string PhoneNumber { get; set; }
            public  string FaxNumber { get; set; }
            public  string Image { get; set; }

            public override string  ToString() {
 	            StringBuilder sb = new StringBuilder();

                sb.Append("<div class=\"webPageContent\">");
                sb.AppendFormat("<div><a href=\"{0}\" class=\"webPageLink\">{1}</a></div>", Url, Title);
                if (!string.IsNullOrWhiteSpace(Description)) {
                    sb.AppendFormat("<div><p class=\"webPageDescription\">{0}</p></div>", Description);
                }
                sb.Append("</div>");

                return sb.ToString();
            }
        }

        public string GetContent(System.Net.HttpWebResponse response) {
            if (!String.IsNullOrEmpty(response.ContentType) &&
                _allowedMimeTypes.Any(mt=> response.ContentType.Contains(mt))) {

                PageDetails details = new PageDetails();
                details.Url = details.Title = response.ResponseUri.ToString();

                // parse with HAP (Html Agility Pack)
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(ReadPage(response));

                // parse head
                var headNode = document.DocumentNode.SelectSingleNode("html/head");
                foreach (var childNode in headNode.ChildNodes) {
                    switch (childNode.Name.ToLower()) {
                        case "title":
                            if (!string.IsNullOrWhiteSpace(childNode.InnerText)) {
                                details.Title = HttpUtility.HtmlDecode(childNode.InnerText);
                            }
                            break;
                        case "meta":
                            if (childNode.Attributes["name"] != null && childNode.Attributes["content"] != null) {
                                switch (childNode.Attributes["name"].Value.ToLower()) {
                                    case "description":
                                        details.Description = HttpUtility.HtmlDecode(childNode.Attributes["content"].Value);
                                        break;
                                }
                            }
                            else if (childNode.Attributes["property"] != null && childNode.Attributes["content"] != null) {
                                // parse open graph
                                switch (childNode.Attributes["property"].Value.ToLower()) {
                                    case "og:title":
                                        details.Title = HttpUtility.HtmlDecode(childNode.Attributes["content"].Value);
                                        break;
                                    case "og:type":
                                        details.Type = childNode.Attributes["content"].Value;
                                        break;
                                    case "og:url":
                                        details.Url = childNode.Attributes["content"].Value;
                                        break;
                                    case "og:image":
                                        details.Image = childNode.Attributes["content"].Value;
                                        break;
                                    case "og:site_name":
                                        details.SiteName = HttpUtility.HtmlDecode(childNode.Attributes["content"].Value);
                                        break;
                                    case "og:description":
                                        details.Description = HttpUtility.HtmlDecode(childNode.Attributes["content"].Value);
                                        break;
                                    case "og:email":
                                        details.Email = HttpUtility.HtmlDecode(childNode.Attributes["content"].Value);
                                        break;
                                    case "og:phone_number":
                                        details.PhoneNumber = HttpUtility.HtmlDecode(childNode.Attributes["content"].Value);
                                        break;
                                    case "og:fax_number":
                                        details.FaxNumber = HttpUtility.HtmlDecode(childNode.Attributes["content"].Value);
                                        break;
                                }
                            }
                            break;
                    }
                }
                return details.ToString();
            }
            return null;
        }
    }
}