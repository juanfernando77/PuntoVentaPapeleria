using PapeleriaAPI.Models;

namespace PapeleriaAPI.Repositories
{
    public interface IVentaRepository
    {
        Task<IEnumerable<Venta>> GetAll();
        Task<IEnumerable<Venta>> GetByFecha(DateTime fecha);
        Task<IEnumerable<Venta>> GetByPeriodo(DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<Venta>> GetVentasHoy();
        Task<Venta?> GetById(int id);
        Task<Venta> Create(Venta venta, List<DetalleVenta> detalles);
        Task<bool> CancelarVenta(int id);
        Task<string> GenerarNumeroVenta();
        Task<decimal> GetTotalVentasPorFecha(DateTime fecha);
        Task<int> GetCantidadVentasPorFecha(DateTime fecha);
    }
}