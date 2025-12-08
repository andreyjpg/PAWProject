using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PAWProject.Core.Business;
using PAWProject.DTOs.DTOs;
using PAWProject.Models.Entities;
using System.Text.Json;

namespace PAWProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SourceController(ISourceBusiness sourceBusiness) : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<Source>> GetSources([FromQuery] int? id)
        {
            return await sourceBusiness.GetSources(id);
        }

        [HttpPost]
        public async Task<bool> SaveSource([FromBody] string source)
        {
            var sourceData = JsonSerializer.Deserialize<Source>(source);
            return await sourceBusiness.SaveSourceAsync(sourceData);
        }


    }
}
