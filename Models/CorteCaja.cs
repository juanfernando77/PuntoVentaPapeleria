namespace PapeleriaAPI.Models
{
    public class CorteCaja
    {
        public int IdCorte { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaApertura { get; set; } = DateTime.Now;
        public DateTime? FechaCierre { get; set; }
        public decimal MontoInicial { get; set; }
        public decimal VentasEfectivo { get; set; }
        public decimal VentasTarjeta { get; set; }
        public decimal VentasTransferencia { get; set; }
        public decimal TotalVentas { get; set; }
        public decimal MontoFinal { get; set; }
        public decimal Diferencia { get; set; }
        public string? Observaciones { get; set; }
        public bool Cerrado { get; set; } = false;

        // Relación
        public Usuario? Usuario { get; set; }
    }
}