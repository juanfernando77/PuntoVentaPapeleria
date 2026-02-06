using System.ComponentModel.DataAnnotations;

namespace PapeleriaAPI.DTOs
{
    public class AbrirCajaRequest
    {
        [Required(ErrorMessage = "El monto inicial es requerido")]
        [Range(0, double.MaxValue, ErrorMessage = "El monto inicial debe ser mayor o igual a 0")]
        public decimal MontoInicial { get; set; }
    }
}