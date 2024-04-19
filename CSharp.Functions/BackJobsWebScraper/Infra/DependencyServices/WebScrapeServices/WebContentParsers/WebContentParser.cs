using AngleSharp;
using AngleSharp.Html.Parser;
using Common;
using Common.Entities;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BackJobsWebScraper.Infra
{
    public class WebContentParser : IWebContentParser
    {
        public StructuredContent GetStructuredWebContent(WebScrapeRun webScrapeRun, List<string> namesForWarning)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(webScrapeRun.HtmlContent);

            var textContent = new TextContent { Text = FormatText(htmlDoc.DocumentNode.OuterHtml) };
            textContent.Text = FlagWordsWithWarning(textContent.Text ?? "", namesForWarning);

            var content = new StructuredContent
            {
                ScrapeRunId = webScrapeRun.Id,
                TextContent = textContent,
                Images = ExtractImages(htmlDoc),
                Videos = ExtractVideos(htmlDoc),
            };

            return content;
        }

        private static List<ImageContent> ExtractImages(HtmlDocument htmlDoc)
        {
            var imageUrls = new List<string>();
            var imgUrls = htmlDoc.DocumentNode.SelectNodes("//img")?
                .Select(e => e.GetAttributeValue("src", null))
                .Where(s => !string.IsNullOrEmpty(s));

            if (imgUrls != null)
            {
                imageUrls.AddRange(imgUrls);
            }

            // TODO: Lookup for other image source like background;
            var images = htmlDoc.DocumentNode.SelectNodes("//*[contains(@style, 'background-image')]");

            if (images == null || !images.Any())
            {
                return imageUrls.Select(i => new ImageContent { Url = i }).ToList();
            }

            foreach (var image in images)
            {
                var source = image.Attributes["style"].Value;
                const string pattern = @"url\('?(.*?)'?\)";
                var match = Regex.Match(source, pattern);
                if (!match.Success)
                {
                    continue;
                }

                var url = match.Groups[1].Value;
                imageUrls.Add(url);
            }

            return imageUrls.Select(i => new ImageContent { Url = i }).ToList();
        }

        private static List<VideoContent> ExtractVideos(HtmlDocument doc)
        {
            var videoUrls = new List<string?>();
            var videoNodes = doc.DocumentNode.SelectNodes("//video | //iframe");

            if (videoNodes == null)
            {
                return videoUrls.Select(i => new VideoContent { Url = i }).ToList();
            }

            foreach (var node in videoNodes)
            {
                string? videoUrl = null;
                if (node.Name.Equals("video", StringComparison.OrdinalIgnoreCase))
                {
                    videoUrl = node.GetAttributeValue("src", null);
                }
                else if (node.Name.Equals("iframe", StringComparison.OrdinalIgnoreCase))
                {
                    videoUrl = node.GetAttributeValue("src", null);
                }

                if (!string.IsNullOrEmpty(videoUrl))
                {
                    videoUrls.Add(videoUrl);
                }
            }

            return videoUrls.Select(i => new VideoContent { Url = i }).ToList();
        }

        private static string FlagWordsWithWarning(string input, ICollection<string> names)
        {
            var pattern = "\\b(" + string.Join("|", names.Select(Regex.Escape)) + ")\\b";
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);

            var flaggedText = regex.Replace(input, match =>
            {
                var word = match.Value;
                return names.Contains(word.ToLower()) ? $"<warn>{match.Value}</warn>" : word;
            });

            return flaggedText;
        }

        //Adjust text formatting to be more readable
        private static string FormatText(string baseText)
        {
            //Add next line marker, etc.
            var formattedText = new HtmlParser()
                .ParseFragment(baseText, null!)
                .ToHtml(new TextMarkupFormatter())
                .Trim();

            formattedText = Regex.Replace(formattedText, @"\s+", " ").Trim();

            // Replace marker
            formattedText = Regex.Replace(formattedText, @"(<br/>\s*)+", "<br/>");

            return formattedText;
        }
    }
}