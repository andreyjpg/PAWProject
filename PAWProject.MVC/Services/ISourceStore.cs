using System.Collections.Generic;
using PAWProject.MVC.Models;

namespace PAWProject.MVC.Services
{
    
    public interface ISourceStore
    {
        IReadOnlyList<Source> GetAll();
        Source? GetById(int id);
        void Add(Source source);
    }
}