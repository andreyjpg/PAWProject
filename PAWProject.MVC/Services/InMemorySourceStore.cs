using System.Collections.Generic;
using System.Linq;
using PAWProject.MVC.Models;

namespace PAWProject.MVC.Services
{

    public class InMemorySourceStore : ISourceStore
    {
        private readonly List<Source> _sources = new();
        private int _idSequence = 1;

        public InMemorySourceStore()
        {
            _sources.Add(new Source
            {
                Id = _idSequence++,
                Url = "https://feeds.bbci.co.uk/news/rss.xml",
                Name = "BBC News (RSS)",
                Description = "Feed principal de noticias de BBC.",
                ComponentType = "feed",
                RequiresSecret = false
            });

            _sources.Add(new Source
            {
                Id = _idSequence++,
                Url = "https://hnrss.org/frontpage",
                Name = "Hacker News Frontpage",
                Description = "Noticias principales de Hacker News (RSS).",
                ComponentType = "feed",
                RequiresSecret = false
            });
        }

        public IReadOnlyList<Source> GetAll() => _sources;

        public Source? GetById(int id) => _sources.FirstOrDefault(s => s.Id == id);

        public void Add(Source source)
        {
            source.Id = _idSequence++;
            _sources.Add(source);
        }
    }
}