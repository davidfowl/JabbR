using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignalR.Samples.Hubs.Chat;
using HtmlAgilityPack;
using System.Net;
using System.IO;
using System.Text;
using System.Drawing;

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
            public  ImageLink Image { get; set; }
            public List<ImageLink> Images { get; set; }

            public PageDetails() {
                this.Images = new List<ImageLink>();
            }

            public override string  ToString() {
 	            StringBuilder sb = new StringBuilder();

                ImageLink firstValidImage = null;
                if (!Image.IsValid) {
                    firstValidImage = Images.FirstOrDefault(img => img.IsValid);
                }
                else {
                    firstValidImage = Image;
                }

                sb.Append("<div class=\"webPageContent\">");
                if (firstValidImage != null) {
                    sb.AppendFormat("<div><img src=\"{0}\" alt=\"{1}\" /></div>", firstValidImage.Href, Title);
                }
                sb.AppendFormat("<div><a href=\"{0}\" class=\"webPageLink\">{1}</a></div>", Url, Title);
                if (!string.IsNullOrWhiteSpace(Description)) {
                    sb.AppendFormat("<div><p class=\"webPageDescription\">{0}</p></div>", Description);
                }
                sb.Append("</div>");

                return sb.ToString();
            }
        }

        private class ImageLink {
            public string Href { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public bool IsValid { get { return Width > 0 && Height > 0; } }

            public ImageLink() { }
            public ImageLink(string pageUrl, string imgUrl) {
                SetImageLink(FullyQualifiedImage(imgUrl, pageUrl));
            }
            public ImageLink(string imgUrl) {
                SetImageLink(imgUrl);
            }

            private void SetImageLink(string url) {
                this.Href = url;
                try {
                    using (WebClient webClient = new WebClient()) {
                        using (MemoryStream stream = new MemoryStream(webClient.DownloadData(url))) {
                            Image img = Image.FromStream(stream);
                            stream.Close();
                            this.Width = img.Width;
                            this.Height = img.Height;
                        }
                    }
                }
                catch {
                    // Swallow
                }
            }

            private string FullyQualifiedImage(string imageUrl, string siteUrl) {
                if (imageUrl.ToLower().StartsWith("http:") || imageUrl.ToLower().StartsWith("https:")) {
                    return imageUrl;
                }

                if (imageUrl.IndexOf("//") == 0) {
                    return "http:" + imageUrl;
                }
                try {
                    string baseurl = siteUrl.Replace("http://", string.Empty).Replace("https://", string.Empty);
                    baseurl = baseurl.Split('/')[0];
                    return string.Format("http://{0}{1}", baseurl, imageUrl);
                }
                catch { }

                return imageUrl;

            }

        }

        //score the image based on matches in comparing alt to title and h1 tag
        private int ScoreImage(string text, string compare) {
            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(compare))
                return 0;
            text = text.Replace("\r\n", string.Empty).Replace("\t", string.Empty);
            compare = compare.Replace("\r\n", string.Empty).Replace("\t", string.Empty);
            int score = 0;
            if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(compare)) {
                string[] c = compare.Split(' ');

                foreach (string test in c) {
                    if (text.Contains(test)) { score++; }
                }
            }
            return score;
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
                                        details.Image = new ImageLink(response.ResponseUri.ToString(), childNode.Attributes["content"].Value);
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

                // Find images
                var imgNodes = document.DocumentNode.SelectNodes("//img");
                var h1Node = document.DocumentNode.SelectSingleNode("//h1");
                string h1Text = null;
                if (h1Node != null && !string.IsNullOrWhiteSpace(h1Node.InnerText)) {
                    h1Text = h1Node.InnerText;
                }
                int bestScore = -1;
                if (imgNodes != null) {
                    foreach (var imageNode in imgNodes) {
                        if (imageNode != null && imageNode.Attributes["src"] != null && imageNode.Attributes["alt"] != null) {
                            ImageLink imgLink = new ImageLink(response.ResponseUri.ToString(), imageNode.Attributes["src"].Value);
                            if (imgLink.IsValid && imgLink.Width > 50) {
                                int imgScore = ScoreImage(imageNode.Attributes["alt"].Value, details.Title);
                                imgScore += ScoreImage(imageNode.Attributes["alt"].Value, h1Text);
                                if (imgScore > bestScore) {
                                    bestScore = imgScore;
                                    details.Image = imgLink;
                                }
                                if (!details.Images.Any(il=> object.ReferenceEquals(imgLink, il) || il.Href.ToLower() == imgLink.Href.ToLower())) {
                                    details.Images.Add(imgLink);
                                }
                            }
                        }
                    }
                }

                return details.ToString();
            }
            return null;
        }
    }
}