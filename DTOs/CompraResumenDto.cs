namespace PapeleriaAPI.DTOs
{
    public class CompraResumenDto
    {
        public int IdCompra { get; set; }
        public string NumeroCompra { get; set; } = string.Empty;
        public string NombreProveedor { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public DateTime FechaCompra { get; set; }
        public decimal Total { get; set; }
        public int CantidadProductos { get; set; }
    }
}