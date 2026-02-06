using System.ComponentModel.DataAnnotations;

namespace PapeleriaAPI.DTOs
{
    public class CerrarCajaRequest
    {
        [Required(ErrorMessage = "El monto final es requerido")]
        [Range(0, double.MaxValue, ErrorMessage = "El monto final debe ser mayor o igual a 0")]
        public decimal MontoFinal { get; set; }

        [StringLength(1000, ErrorMessage = "Las observaciones no pueden exceder 1000 caracteres")]
        public string? Observaciones { get; set; }
    }
}