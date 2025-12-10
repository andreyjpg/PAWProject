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
    public class SourceItemController(ISourceItemBusiness sourceItemBusiness) : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<SourceItemDTO>> GetSourceItems([FromQuery] int? id)
        {
            return sourceItemBusiness.GetSourceItems(id)
                .Result
                .Select(item => new SourceItemDTO
                {
                    Id = item.Id,
                    SourceId = item.SourceId,
                    Json = item.Json,
                    CreatedAt = item.CreatedAt
                });
        }

        [HttpPost]
        public async Task<IActionResult> SaveSourceItem([FromBody] string itemDto)
        {
            var sourceItem = JsonSerializer.Deserialize<SourceItem>(itemDto);
            var result = await sourceItemBusiness.SaveSourceItemAsync(sourceItem);
            if (result)
                return Ok();
            return BadRequest();
        }

    }
}
