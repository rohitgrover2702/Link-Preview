using HtmlAgilityPack;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;

namespace LinkPreview
{
    public class LinkPreview
    {
        public MetaTagModel GetMetaTagValueUrl(string url)
        {
            MetaTagModel metaObj = new MetaTagModel();
            try
            {
                if (url != null && CheckURlPingStatus(url))
                {
                    url = url.Contains("http") ? url : "http://" + url;
                    string RegexPattern = @"youtu(?:\.be|be\.com)/(?:(.*)v(/|=)|(.*/)?)([a-zA-Z0-9-_]+)";
                    System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(RegexPattern);
                    System.Text.RegularExpressions.Match match = regex.Match(url);
                    if (match.Success)
                    {

                        var api = $"http://youtube.com/get_video_info?video_id={GetArgs(url, "v", '?')}";
                        var resultObj = GetArgs(new WebClient().DownloadString(api), "title", '&');
                        metaObj.Title = resultObj != null ? resultObj : null;
                        Uri urlobj = new Uri(url);
                        var query = HttpUtility.ParseQueryString(urlobj.Query);
                        String vedioID = String.Empty;

                        if (query.AllKeys.Contains("v"))
                        {
                            vedioID = System.Text.RegularExpressions.Regex.Match(query["v"], @"^[a-zA-Z0-9_-]{11}$").Value;
                        }
                        metaObj.ImageUrl = vedioID != null ? "https://i.ytimg.com/vi/" + vedioID + "/hqdefault.jpg" : null;
                        metaObj.IsVideoUrl = true;
                        metaObj.DomainName = urlobj.Host != null ? urlobj.Host : null;
                        metaObj.Url = url;
                        metaObj.VideoId = vedioID != null ? vedioID : null;
                        //response.responseData = metaObj;
                        //response.status = 1;
                        //response.message = Constants.Retreived;
                    }
                    else
                    {
                        var getHtmlDoc = new HtmlWeb();
                        var document = getHtmlDoc.Load(url);
                        var metaTags = document.DocumentNode.SelectNodes("//meta");
                        Uri myUri = new Uri(url);
                        metaObj.DomainName = myUri.Host != null ? myUri.Host : null;


                        if (metaTags != null)
                        {
                            foreach (var tag in metaTags)
                            {
                                var tagName = tag.Attributes["name"];
                                var tagContent = tag.Attributes["content"];
                                var tagProperty = tag.Attributes["property"];
                                if (tagName != null && tagContent != null)
                                {
                                    switch (tagName.Value.ToLower())
                                    {
                                        case "title":
                                            metaObj.Title = tagContent.Value;
                                            break;
                                        case "description":
                                            metaObj.Description = tagContent.Value;
                                            break;
                                        case "twitter:title":
                                            metaObj.Title = string.IsNullOrEmpty(metaObj.Title) ? tagContent.Value : metaObj.Title;
                                            break;
                                        case "twitter:description":
                                            metaObj.Description = string.IsNullOrEmpty(metaObj.Description) ? tagContent.Value : metaObj.Description;
                                            break;
                                        case "twitter:image":
                                            metaObj.ImageUrl = string.IsNullOrEmpty(metaObj.ImageUrl) ? tagContent.Value : metaObj.ImageUrl;
                                            break;
                                    }
                                }
                                else if (tagProperty != null)
                                {
                                    switch (tagProperty.Value.ToLower())
                                    {
                                        case "og:title":
                                            metaObj.Title = string.IsNullOrEmpty(metaObj.Title) ? tagContent.Value : metaObj.Title;
                                            break;
                                        case "og:description":
                                            metaObj.Description = string.IsNullOrEmpty(metaObj.Description) ? tagContent.Value : metaObj.Description;
                                            break;
                                        case "og:image":
                                            metaObj.ImageUrl = string.IsNullOrEmpty(metaObj.ImageUrl) ? tagContent.Value : metaObj.ImageUrl;
                                            break;
                                    }
                                }
                                else if (tagContent != null)
                                {
                                    switch (tagContent.Value.ToLower())
                                    {
                                        case "og:title":
                                            metaObj.Title = string.IsNullOrEmpty(metaObj.Title) ? tagContent.Value : metaObj.Title;
                                            break;
                                        case "og:description":
                                            metaObj.Description = string.IsNullOrEmpty(metaObj.Description) ? tagContent.Value : metaObj.Description;
                                            break;
                                        case "og:image":
                                            metaObj.ImageUrl = string.IsNullOrEmpty(metaObj.ImageUrl) ? tagContent.Value : metaObj.ImageUrl;
                                            break;
                                    }
                                }

                            }

                        }
                        var description = document.DocumentNode.SelectSingleNode("//meta[@name='description']");
                        var title = document.DocumentNode.SelectSingleNode("html/head/title");
                        metaObj.Description = ((metaObj.Description == String.Empty || metaObj.Description == null) && description != null) ? description.ToString() : metaObj.Description;
                        metaObj.Url = (metaObj.Url == string.Empty || metaObj.Url == null) ? url : metaObj.Url;
                        metaObj.Title = ((metaObj.Title == string.Empty || metaObj.Title == null) && title != null) ? title.InnerText.ToString() : metaObj.Title;
                        // For converting Image and url from http to https
                        metaObj.Url = string.IsNullOrEmpty(metaObj.Url) ? string.Empty : metaObj.Url.Contains("http") ? metaObj.Url.Replace("http://", "https://") : metaObj.Url;
                        metaObj.ImageUrl = string.IsNullOrEmpty(metaObj.ImageUrl) ? string.Empty : metaObj.ImageUrl.Contains("http") ? metaObj.ImageUrl.Replace("http://", "https://") : metaObj.ImageUrl;
                    }

                }
                else
                {
                    metaObj = null;
                }
            }
            catch (Exception ex)
            {

            }
            return metaObj;
        }

        private bool CheckURlPingStatus(string url)
        {
            try
            {
                string RegexPattern = @"^[0-9]\d*(\.\d+)?$";
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(RegexPattern);
                System.Text.RegularExpressions.Match match = regex.Match(url);
                if (match.Success || url == "http://services.jeddah.gov.sa")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        private static string GetArgs(string args, string key, char query)
        {
            var iqs = args.IndexOf(query);
            NameValueCollection qscoll = HttpUtility.ParseQueryString(args);
            return iqs == -1
                ? string.Empty
                : HttpUtility.ParseQueryString(iqs < args.Length - 1
                    ? args.Substring(iqs + 1) : string.Empty)[key];
        }
    }

    public class MetaTagModel
    {
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public bool IsVideoUrl { get; set; }
        public string DomainName { get; set; }
        public string Url { get; set; }
        public string VideoId { get; set; }
        public string Description { get; set; }
    }
}
