using System.ComponentModel.DataAnnotations;

namespace PapeleriaAPI.DTOs
{
    public class CreateProveedorRequest
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(13, MinimumLength = 12, ErrorMessage = "El RFC debe tener entre 12 y 13 caracteres")]
        [RegularExpression(@"^[A-ZÑ&]{3,4}\d{6}[A-Z0-9]{3}$", ErrorMessage = "Formato de RFC inválido")]
        public string? RFC { get; set; }

        [Phone(ErrorMessage = "Formato de teléfono inválido")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
        public string? Telefono { get; set; }

        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [StringLength(150, ErrorMessage = "El email no puede exceder 150 caracteres")]
        public string? Email { get; set; }

        [StringLength(500, ErrorMessage = "La dirección no puede exceder 500 caracteres")]
        public string? Direccion { get; set; }
    }
}