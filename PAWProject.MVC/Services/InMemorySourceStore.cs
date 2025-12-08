using System.Collections.Generic;
using System.Linq;
using PAWProject.DTOs.DTOs;
using PAWProject.MVC.Models;

namespace PAWProject.MVC.Services
{

    public class InMemorySourceStore : ISourceStore
    {
        private readonly List<SourceDTO> _sources = new();
        private int _idSequence = 1;

        public InMemorySourceStore()
        {
            _sources.Add(new SourceDTO
            {
                Id = _idSequence++,
                Url = "https://feeds.bbci.co.uk/news/rss.xml",
                Name = "BBC News (RSS)",
                Description = "Feed principal de noticias de BBC.",
                ComponentType = "feed",
                RequiresSecret = false
            });

            _sources.Add(new SourceDTO
            {
                Id = _idSequence++,
                Url = "https://hnrss.org/frontpage",
                Name = "Hacker News Frontpage",
                Description = "Noticias principales de Hacker News (RSS).",
                ComponentType = "feed",
                RequiresSecret = false
            });
        }

        public IReadOnlyList<SourceDTO> GetAll() => _sources;

        public SourceDTO? GetById(int id) => _sources.FirstOrDefault(s => s.Id == id);

        public void Add(SourceDTO source)
        {
            source.Id = _idSequence++;
            _sources.Add(source);
        }
    }
}