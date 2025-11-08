using PAWproject.RSSConsumer.Models;
using System.ServiceModel.Syndication;
using System.Xml;

public class RSSConsumer
{
    private readonly string _feedUrl;
    public RSSConsumer(string feedUrl)
    {
        _feedUrl = feedUrl;
    }
    public async Task<IEnumerable<FeedItem>> GetFeedItemsAsync(string? category, string? location)
    {

        var categoryUrl = category != null ? $"/{category}" : "";
        var locationUrl = location != null ? $"/{location}" : "";

        var reader = XmlReader.Create($"{_feedUrl}{categoryUrl}{locationUrl}/rss.xml");
        var feed = SyndicationFeed.Load(reader);

        return feed.Items.Select(item => new FeedItem
        {
            Id = item.Id,
            Title = item.Title.Text,
            Uri = item.Links.FirstOrDefault()?.Uri.ToString(),
            PublishDate = item.PublishDate.UtcDateTime,
            Description = item.Summary?.Text,
            Image = item.ElementExtensions
             .Where(e => e.OuterName == "thumbnail" && e.OuterNamespace == "http://search.yahoo.com/mrss/")
             .Select(e => e.GetObject<XmlElement>()?.GetAttribute("url"))
             .FirstOrDefault()

        });

    }
}