namespace PapeleriaAPI.Models
{
    public class DetalleVenta
    {
        public int IdDetalleVenta { get; set; }
        public int IdVenta { get; set; }
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal Subtotal { get; set; }
        public Venta? Venta { get; set; }
        public Producto? Producto { get; set; }
    }
}