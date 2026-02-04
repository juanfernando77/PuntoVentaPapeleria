namespace PapeleriaAPI.DTOs
{
    public class VentaResumenDto
    {
        public int IdVenta { get; set; }
        public string NumeroVenta { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public DateTime FechaVenta { get; set; }
        public decimal Total { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public int CantidadProductos { get; set; }
    }
}
