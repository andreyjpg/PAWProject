using PAWProject.DTOs.DTOs;
using PAWProject.MVC.Models;
using System.Collections.Generic;

namespace PAWProject.MVC.Services
{
    
    public interface ISourceStore
    {
        IReadOnlyList<SourceDTO> GetAll();
        SourceDTO? GetById(int id);
        void Add(SourceDTO source);
    }
}