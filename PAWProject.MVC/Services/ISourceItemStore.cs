using System.Collections.Generic;
using PAWProject.MVC.Models;

namespace PAWProject.MVC.Services
{
   
    public interface ISourceItemStore
    {
        IReadOnlyList<SourceItem> GetAll();
        IReadOnlyList<SourceItem> GetBySourceId(int sourceId);
        void Add(SourceItem item);
    }
}