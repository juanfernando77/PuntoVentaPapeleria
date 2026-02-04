namespace PapeleriaAPI.DTOs
{
    public class DetalleVentaDto
    {
        public int IdDetalleVenta { get; set; }
        public int IdProducto { get; set; }
        public string CodigoProducto { get; set; } = string.Empty;
        public string NombreProducto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal Subtotal { get; set; }
    }
}