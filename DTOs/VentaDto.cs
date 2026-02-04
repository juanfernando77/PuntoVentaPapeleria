namespace PapeleriaAPI.DTOs
{
    public class VentaDto
    {
        public int IdVenta { get; set; }
        public string NumeroVenta { get; set; } = string.Empty;
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public DateTime FechaVenta { get; set; }
        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public decimal MontoPagado { get; set; }
        public decimal Cambio { get; set; }
        public int CantidadProductos { get; set; }
        public List<DetalleVentaDto> Detalles { get; set; } = new();
    }
}