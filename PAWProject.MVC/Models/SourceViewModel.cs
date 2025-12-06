using PAWProject.DTOs;
using PAWProject.DTOs.DTOs;
using System.Collections.Generic;

namespace PAWProject.MVC.Models
{
   
    public class SourceViewModel
    {

        public IEnumerable<SourceDTO>? Sources { get; set; }

        public SourceDTO? NewSource { get; set; } = new SourceDTO();
    }
}