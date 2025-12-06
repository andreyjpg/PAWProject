using System.Collections.Generic;
using System.Threading.Tasks;
using PAWProject.DTOs;
using PAWProject.DTOs.DTOs;
using PAWProject.MVC.Models;

namespace PAWProject.MVC.Services
{
   
    public interface INewsIngestionService
    {
        Task<List<FeedItemDTO>> GetItemsFromSourceAsync(SourceDTO source);
    }
}