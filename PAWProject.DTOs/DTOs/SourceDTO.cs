using System.ComponentModel.DataAnnotations;

namespace PAWProject.DTOs.DTOs
{
    public class SourceDTO
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "La URL debe de ir si o si.")]
        [StringLength(500, ErrorMessage = "La URL supera la cantidad de caracteres permitidos.")]
        [Display(Name = "URL de la fuente")]
        [Url(ErrorMessage = "Ingrese una URL válida (incluya http:// o https://).")]
        public string Url { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre tiene que ir si o si.")]
        [StringLength(200, ErrorMessage = "El nombre supera la cantidad de caracteres permitidos.")]
        [Display(Name = "Nombre")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción supera la cantidad de caracteres permitidos.")]
        [Display(Name = "Descripcion")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "El tipo de componente es obligatorio.")]
        [StringLength(100, ErrorMessage = "El tipo de componente supera la cantidad de caracteres permitidos.")]
        [Display(Name = "Tipo de componente")]
        public string ComponentType { get; set; } = "feed";

        [Display(Name = "Requiere secreto API key")]
        public bool RequiresSecret { get; set; }

        public virtual ICollection<SecretDTO> Secrets { get; set; } = new List<SecretDTO>();

        public virtual ICollection<SourceItemDTO> SourceItems { get; set; } = new List<SourceItemDTO>();
    }
}
