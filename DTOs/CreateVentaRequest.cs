using System.ComponentModel.DataAnnotations;

namespace PapeleriaAPI.DTOs
{
    public class CreateVentaRequest
    {
        [Required(ErrorMessage = "Los items son requeridos")]
        [MinLength(1, ErrorMessage = "Debe agregar al menos un producto")]
        public List<ItemVentaRequest> Items { get; set; } = new();

        [Required(ErrorMessage = "El método de pago es requerido")]
        [RegularExpression("^(Efectivo|Tarjeta|Transferencia)$",
            ErrorMessage = "Método de pago inválido. Opciones: Efectivo, Tarjeta, Transferencia")]
        public string MetodoPago { get; set; } = "Efectivo";

        [Required(ErrorMessage = "El monto pagado es requerido")]
        [Range(0, double.MaxValue, ErrorMessage = "El monto pagado debe ser mayor o igual a 0")]
        public decimal MontoPagado { get; set; }

        public bool AplicarIVA { get; set; } = true; // Si se debe aplicar IVA (16%)
    }
}