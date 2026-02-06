namespace PapeleriaAPI.DTOs
{
    public class CorteCajaDto
    {
        public int IdCorte { get; set; }
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public DateTime FechaApertura { get; set; }
        public DateTime? FechaCierre { get; set; }
        public decimal MontoInicial { get; set; }
        public decimal VentasEfectivo { get; set; }
        public decimal VentasTarjeta { get; set; }
        public decimal VentasTransferencia { get; set; }
        public decimal TotalVentas { get; set; }
        public decimal MontoFinal { get; set; }
        public decimal Diferencia { get; set; }
        public string? Observaciones { get; set; }
        public bool Cerrado { get; set; }
        public int CantidadVentas { get; set; }
        public TimeSpan? TiempoAbierto { get; set; }
    }
}