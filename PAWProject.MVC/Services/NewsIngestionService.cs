using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using PAWProject.DTOs;
using PAWProject.MVC.Models;

namespace PAWProject.MVC.Services
{
    
    public class NewsIngestionService : INewsIngestionService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public NewsIngestionService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<FeedItemDTO>> GetItemsFromSourceAsync(Source source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var client = _httpClientFactory.CreateClient();
            using var response = await client.GetAsync(source.Url);
            response.EnsureSuccessStatusCode();

            var mediaType = response.Content.Headers.ContentType?.MediaType ?? string.Empty;
            var content = await response.Content.ReadAsStringAsync();

            if (mediaType.Contains("json", StringComparison.OrdinalIgnoreCase) ||
                source.ComponentType.Equals("api", StringComparison.OrdinalIgnoreCase))
            {
                return ParseJson(content);
            }

            if (mediaType.Contains("xml", StringComparison.OrdinalIgnoreCase) ||
                mediaType.Contains("rss", StringComparison.OrdinalIgnoreCase) ||
                mediaType.Contains("atom", StringComparison.OrdinalIgnoreCase) ||
                source.ComponentType.Equals("feed", StringComparison.OrdinalIgnoreCase))
            {
                return ParseRssOrAtom(content);
            }

            return ParseHtmlAsSingleItem(source, content);
        }

        private List<FeedItemDTO> ParseRssOrAtom(string xmlContent)
        {
            using var stringReader = new System.IO.StringReader(xmlContent);
            using var xmlReader = XmlReader.Create(stringReader);
            var feed = SyndicationFeed.Load(xmlReader);

            if (feed == null)
            {
                return new List<FeedItemDTO>();
            }

            return feed.Items.Select(item => new FeedItemDTO
            {
                Id = item.Id,
                Title = item.Title?.Text,
                Description = item.Summary?.Text,
                PublishDate = item.PublishDate.UtcDateTime,
                Uri = item.Links.FirstOrDefault()?.Uri.ToString(),
                Image = null 
            }).ToList();
        }

        private List<FeedItemDTO> ParseJson(string json)
        {
            var result = new List<FeedItemDTO>();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            IEnumerable<JsonElement> elements;

            if (root.ValueKind == JsonValueKind.Array)
            {
                elements = root.EnumerateArray();
            }
            else
            {
                if (root.TryGetProperty("items", out var itemsProp) && itemsProp.ValueKind == JsonValueKind.Array)
                    elements = itemsProp.EnumerateArray();
                else if (root.TryGetProperty("articles", out var articlesProp) && articlesProp.ValueKind == JsonValueKind.Array)
                    elements = articlesProp.EnumerateArray();
                else if (root.TryGetProperty("data", out var dataProp) && dataProp.ValueKind == JsonValueKind.Array)
                    elements = dataProp.EnumerateArray();
                else
                    elements = Array.Empty<JsonElement>();
            }

            foreach (var el in elements)
            {
                string? title = TryGetString(el, "title", "name", "headline");
                string? description = TryGetString(el, "description", "summary", "body");
                string? uri = TryGetString(el, "url", "link", "uri");
                DateTime? publishDate = TryGetDate(el, "publishedAt", "date", "pubDate");

                result.Add(new FeedItemDTO
                {
                    Id = uri ?? title,
                    Title = title,
                    Description = description,
                    Uri = uri,
                    PublishDate = publishDate,
                    Image = TryGetString(el, "imageUrl", "image", "thumbnail")
                });
            }

            return result;
        }

        private List<FeedItemDTO> ParseHtmlAsSingleItem(Source source, string html)
        {
            string title = ExtractBetween(html, "<title", "</title>") ?? source.Name;
            var closing = title.IndexOf('>');
            if (closing >= 0 && closing < title.Length - 1)
            {
                title = title[(closing + 1)..];
            }

            string description = StripHtml(html);
            if (description.Length > 200)
            {
                description = description[..200] + "...";
            }

            return new List<FeedItemDTO>
            {
                new FeedItemDTO
                {
                    Id = source.Url,
                    Title = title?.Trim(),
                    Description = description,
                    Uri = source.Url,
                    PublishDate = DateTime.UtcNow
                }
            };
        }

        private static string? ExtractBetween(string text, string startToken, string endToken)
        {
            var startIndex = text.IndexOf(startToken, StringComparison.OrdinalIgnoreCase);
            if (startIndex < 0) return null;
            var fromStart = text.IndexOf('>', startIndex);
            if (fromStart < 0) return null;
            var endIndex = text.IndexOf(endToken, fromStart, StringComparison.OrdinalIgnoreCase);
            if (endIndex < 0) return null;
            return text.Substring(startIndex, endIndex - startIndex + endToken.Length);
        }

        private static string StripHtml(string input)
        {
            var output = new StringBuilder(input.Length);
            bool inside = false;
            foreach (var c in input)
            {
                if (c == '<')
                {
                    inside = true;
                    continue;
                }
                if (c == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    output.Append(c);
                }
            }

            return output.ToString().Replace("\n", " ").Replace("\r", " ");
        }

        private static string? TryGetString(JsonElement el, params string[] names)
        {
            foreach (var name in names)
            {
                if (el.TryGetProperty(name, out var prop) && prop.ValueKind == JsonValueKind.String)
                {
                    return prop.GetString();
                }
            }
            return null;
        }

        private static DateTime? TryGetDate(JsonElement el, params string[] names)
        {
            foreach (var name in names)
            {
                if (el.TryGetProperty(name, out var prop))
                {
                    if (prop.ValueKind == JsonValueKind.String &&
                        DateTime.TryParse(prop.GetString(), out var dt))
                    {
                        return dt;
                    }
                }
            }
            return null;
        }
    }
}