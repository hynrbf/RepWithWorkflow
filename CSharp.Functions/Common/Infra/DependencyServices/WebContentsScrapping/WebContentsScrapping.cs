using Common.Entities;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace Common.Infra
{
    // ref. HTML Agility Pack - https://www.youtube.com/watch?v=oMM0yzyi4Do
    public class WebContentsScrapping : IWebContentsScrapping
    {
        public HtmlNodeCollection? GetNodesFromHtmlByTag(string htmlContent, string tag, string keyword)
        {
            if (string.IsNullOrEmpty(htmlContent))
            {
                return null;
            }

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            if (string.IsNullOrEmpty(keyword))
            {
                return htmlDoc.DocumentNode.SelectNodes($"//{tag}");
            }

            var nodeContainer = htmlDoc.DocumentNode.SelectSingleNode($"//*[contains(text(), '{keyword}')]");
            return nodeContainer.SelectNodes(tag);
        }

        public IEnumerable<string> GetContentFromHtmlByKeyword(string htmlContent, string keyword)
        {
            if (string.IsNullOrEmpty(htmlContent))
            {
                return Enumerable.Empty<string>();
            }

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);
            return GetContent(keyword, htmlDoc);
        }

        public IEnumerable<string> GetContentFromUrlByKeyword(string url, string keyword)
        {
            if (string.IsNullOrEmpty(url))
            {
                return Enumerable.Empty<string>();
            }

            var web = new HtmlWeb();
            var htmlDoc = web.Load(url);
            return GetContent(keyword, htmlDoc);
        }

        private static IEnumerable<string> GetContent(string keyword, HtmlDocument htmlDoc)
        {
            var xPaths = FindElementsByInnerHtml(htmlDoc.DocumentNode, keyword);

            if (!xPaths.Any())
            {
                return Enumerable.Empty<string>();
            }

            var contents = new List<string>();
            Console.WriteLine("XPaths");
            xPaths.ForEach(x =>
            {
                var regex = new Regex(@"#text(?:\[\d+\])?$");

                if (regex.IsMatch(x))
                {
                    return;
                }

                var content = GetContent(htmlDoc.DocumentNode, x);

                if (!(string.IsNullOrEmpty(content.Trim()) || contents.Any(c => c == content)))
                {
                    contents.Add(content);
                }
            });

            return contents;
        }

        public ScrappedWebContent GetScrappedWebContent(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return new ScrappedWebContent();
            }

            try
            {
                var web = new HtmlWeb();
                var htmlDoc = web.Load(url);
                return new ScrappedWebContent
                {
                    Id = Guid.NewGuid().ToString(),
                    Url = url,
                    HtmlContent = RemoveWhiteSpaces(htmlDoc.DocumentNode.OuterHtml)
                };
            }
            catch (Exception ex)
            {
                if (ex.InnerException?.InnerException?.Message.Contains(
                        "Received an unexpected EOF or 0 bytes from the transport stream.") ?? false)
                {
                    // This typically happens if the url is not existing
                    Console.WriteLine($"The {url} might not be existing.");
                    return new ScrappedWebContent();
                }

                Console.WriteLine(
                    $"An error occured at WebScrappingRepository.GetWebScrapItemAndSaveToDbAsync().\n{ex.Message}");
                throw;
            }
        }

        private static string GetContent(HtmlNode node, string xPath)
        {
            var selectedNode = node.SelectNodes(xPath).FirstOrDefault();

            if (selectedNode == null)
            {
                return string.Empty;
            }

            var parentNode = selectedNode.ParentNode;
            var child = parentNode.ChildNodes.Where(n => !string.IsNullOrEmpty(n.InnerHtml.Trim())).ToList();
            var index = child.IndexOf(selectedNode);

            for (var i = index + 1; i < child.Count; i++)
            {
                var innerText = child[i].InnerText?.Trim();

                if (string.IsNullOrEmpty(innerText))
                {
                    continue;
                }

                return RemoveWhiteSpaces(innerText, Environment.NewLine);
            }

            return string.Empty;
        }

        private static List<string> FindElementsByInnerHtml(HtmlNode node, string innerHtml,
            List<string>? xpaths = null)
        {
            var outputXPaths = new List<string>();

            if (node.InnerHtml.Trim() == innerHtml)
            {
                // If inner HTML matches, get the XPath of the element and add it to the list
                outputXPaths.Add(node.XPath);
            }

            foreach (var childNode in node.ChildNodes)
            {
                xpaths ??= new List<string>();

                // Recursively search child nodes
                var childXPaths = FindElementsByInnerHtml(childNode, innerHtml, xpaths);

                if (!childXPaths.Any())
                {
                    continue;
                }

                outputXPaths.AddRange(childXPaths);
            }

            return outputXPaths;
        }

        private static string RemoveWhiteSpaces(string input, string joiningChar = "")
        {
            input = input
                .Replace("\r\n", Environment.NewLine)
                .Replace("\r", string.Empty)
                .Replace("\t", string.Empty)
                .Replace("&nbsp;", " ");
            var lines = input.Split("\n").ToList();
            var linesWithoutWhiteSpaces = lines.Where(line => !string.IsNullOrWhiteSpace(line.Trim()));
            return string.Join(joiningChar, linesWithoutWhiteSpaces);
        }
    }
}