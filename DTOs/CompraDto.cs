namespace PapeleriaAPI.DTOs
{
    public class CompraDto
    {
        public int IdCompra { get; set; }
        public string NumeroCompra { get; set; } = string.Empty;
        public int IdProveedor { get; set; }
        public string NombreProveedor { get; set; } = string.Empty;
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public DateTime FechaCompra { get; set; }
        public decimal Total { get; set; }
        public string? Observaciones { get; set; }
        public int CantidadProductos { get; set; }
        public List<DetalleCompraDto> Detalles { get; set; } = new();
    }
}