namespace PapeleriaAPI.Models
{
    public class Compra
    {
        public int IdCompra { get; set; }
        public string NumeroCompra { get; set; } = string.Empty;
        public int IdProveedor { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaCompra { get; set; } = DateTime.Now;
        public decimal Total { get; set; }
        public string? Observaciones { get; set; }

        // Relaciones
        public Proveedor? Proveedor { get; set; }
        public Usuario? Usuario { get; set; }
        public ICollection<DetalleCompra>? DetallesCompra { get; set; }
    }
}