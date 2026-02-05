using System.ComponentModel.DataAnnotations;

namespace PapeleriaAPI.DTOs
{
    public class CreateCompraRequest
    {
        [Required(ErrorMessage = "El proveedor es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un proveedor válido")]
        public int IdProveedor { get; set; }

        [Required(ErrorMessage = "Los items son requeridos")]
        [MinLength(1, ErrorMessage = "Debe agregar al menos un producto")]
        public List<ItemCompraRequest> Items { get; set; } = new();

        [StringLength(1000, ErrorMessage = "Las observaciones no pueden exceder 1000 caracteres")]
        public string? Observaciones { get; set; }
    }
}