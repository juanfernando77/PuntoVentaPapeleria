using PapeleriaAPI.DTOs;

namespace PapeleriaAPI.Services
{
    public interface ICompraService
    {
        Task<ApiResponse<IEnumerable<CompraResumenDto>>> GetAll();
        Task<ApiResponse<IEnumerable<CompraResumenDto>>> GetByFecha(DateTime fecha);
        Task<ApiResponse<IEnumerable<CompraResumenDto>>> GetByProveedor(int idProveedor);
        Task<ApiResponse<IEnumerable<CompraResumenDto>>> GetByPeriodo(DateTime fechaInicio, DateTime fechaFin);
        Task<ApiResponse<CompraDto>> GetById(int id);
        Task<ApiResponse<CompraDto>> Create(CreateCompraRequest request, int idUsuario);
        Task<ApiResponse<bool>> CancelarCompra(int id);
        Task<ApiResponse<Dictionary<string, object>>> GetEstadisticasHoy();
    }
}