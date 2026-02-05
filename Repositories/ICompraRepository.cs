using PapeleriaAPI.Models;

namespace PapeleriaAPI.Repositories
{
    public interface ICompraRepository
    {
        Task<IEnumerable<Compra>> GetAll();
        Task<IEnumerable<Compra>> GetByFecha(DateTime fecha);
        Task<IEnumerable<Compra>> GetByProveedor(int idProveedor);
        Task<IEnumerable<Compra>> GetByPeriodo(DateTime fechaInicio, DateTime fechaFin);
        Task<Compra?> GetById(int id);
        Task<Compra> Create(Compra compra, List<DetalleCompra> detalles);
        Task<bool> CancelarCompra(int id);
        Task<string> GenerarNumeroCompra();
        Task<decimal> GetTotalComprasPorFecha(DateTime fecha);
        Task<int> GetCantidadComprasPorFecha(DateTime fecha);
    }
}