namespace PapeleriaAPI.Models
{
    public class Venta
    {
        public int IdVenta { get; set; }
        public string NumeroVenta { get; set; } = string.Empty;
        public int IdUsuario { get; set; }
        public DateTime FechaVenta { get; set; } = DateTime.Now;
        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
        public string MetodoPago { get; set; } = "Efectivo"; // Efectivo, Tarjeta, Transferencia
        public decimal MontoPagado { get; set; }
        public decimal Cambio { get; set; }

        // Relaciones
        public Usuario? Usuario { get; set; }
        public ICollection<DetalleVenta>? DetallesVenta { get; set; }
    }
}