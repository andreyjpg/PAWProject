using System;
using System.Collections.Generic;
using System.Linq;
using PAWProject.MVC.Models;

namespace PAWProject.MVC.Services
{
   
    public class InMemorySourceItemStore : ISourceItemStore
    {
        private readonly List<SourceItem> _items = new();
        private int _idSequence = 1;

        public IReadOnlyList<SourceItem> GetAll() => _items;

        public IReadOnlyList<SourceItem> GetBySourceId(int sourceId) =>
            _items.Where(i => i.SourceId == sourceId).ToList();

        public void Add(SourceItem item)
        {
            item.Id = _idSequence++;
            if (item.CreatedAt == default)
            {
                item.CreatedAt = DateTime.UtcNow;
            }
            _items.Add(item);
        }
    }
}