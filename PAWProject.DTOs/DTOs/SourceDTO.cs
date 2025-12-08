using System.ComponentModel.DataAnnotations;

namespace PAWProject.DTOs.DTOs
{
    public class SourceDTO
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "La URL es debe de ir si o si.")]
        [StringLength(500)]
        [Display(Name = "URL de la fuente")]
        public string Url { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre tiene que ir si o si.")]
        [StringLength(200)]
        [Display(Name = "Nombre")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Descripcion")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "El tipo de componente es obligatorio.")]
        [StringLength(100)]
        [Display(Name = "Tipo de componente")]
        public string ComponentType { get; set; } = "feed";

        [Display(Name = "Requiere secreto API key")]
        public bool RequiresSecret { get; set; }

        public virtual ICollection<SecretDTO> Secrets { get; set; } = new List<SecretDTO>();

        public virtual ICollection<SourceItemDTO> SourceItems { get; set; } = new List<SourceItemDTO>();
    }
}
