using PapeleriaAPI.DTOs;

namespace PapeleriaAPI.Services
{
    public interface IVentaService
    {
        Task<ApiResponse<IEnumerable<VentaResumenDto>>> GetAll();
        Task<ApiResponse<IEnumerable<VentaResumenDto>>> GetVentasHoy();
        Task<ApiResponse<IEnumerable<VentaResumenDto>>> GetByFecha(DateTime fecha);
        Task<ApiResponse<IEnumerable<VentaResumenDto>>> GetByPeriodo(DateTime fechaInicio, DateTime fechaFin);
        Task<ApiResponse<VentaDto>> GetById(int id);
        Task<ApiResponse<VentaDto>> Create(CreateVentaRequest request, int idUsuario);
        Task<ApiResponse<bool>> CancelarVenta(int id);
        Task<ApiResponse<Dictionary<string, object>>> GetEstadisticasHoy();
    }
}