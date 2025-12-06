using PAWProject.DTOs;
using PAWProject.DTOs.DTOs;
using PAWProject.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace PAWProject.MVC.Services
{
    
    public class NewsIngestionService : INewsIngestionService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public NewsIngestionService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<FeedItemDTO>> GetItemsFromSourceAsync(SourceDTO source)
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
            using var stringReader = new StringReader(xmlContent);
            using var xmlReader = XmlReader.Create(stringReader);
            var feed = SyndicationFeed.Load(xmlReader);

            if (feed == null)
                return new List<FeedItemDTO>();

            return feed.Items.Select(item =>
            {
                string? imageUrl = null;

                // 1. Intentar obtener imagen desde media:thumbnail o media:content
                try
                {
                    foreach (var ext in item.ElementExtensions)
                    {
                        var ele = ext.GetObject<XElement>();

                        if (ele.Name.LocalName is "thumbnail" or "content")
                        {
                            var url = ele.Attribute("url")?.Value;
                            if (!string.IsNullOrWhiteSpace(url))
                            {
                                imageUrl = url;
                                break;
                            }
                        }
                    }
                }
                catch { /* si falla alguna extensión, ignoramos */ }

                // 2. Si no encontró nada en media, intentar extraer del summary (HTML <img>)
                if (imageUrl == null && item.Summary != null)
                {
                    var match = Regex.Match(item.Summary.Text, "src=[\"'](?<url>.+?)[\"']",
                        RegexOptions.IgnoreCase);

                    if (match.Success)
                        imageUrl = match.Groups["url"].Value;
                }

                // 3. Último fallback: algunos feeds ponen imagen en "Links" con rel="enclosure"
                imageUrl ??= item.Links
                    .FirstOrDefault(l => l.RelationshipType == "enclosure" &&
                                         l.MediaType?.StartsWith("image") == true)
                    ?.Uri.ToString();

                return new FeedItemDTO
                {
                    Id = item.Id,
                    Title = StripHtml(item.Title?.Text),
                    Description = StripHtml(item.Summary?.Text),
                    PublishDate = item.PublishDate.UtcDateTime,
                    Uri = item.Links.FirstOrDefault()?.Uri.ToString(),
                    Image = imageUrl ?? "/Image/default-news.png"
                };
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

        private List<FeedItemDTO> ParseHtmlAsSingleItem(SourceDTO source, string html)
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