using PapeleriaAPI.DTOs;

namespace PapeleriaAPI.Services
{
    public interface ICorteCajaService
    {
        Task<ApiResponse<IEnumerable<CorteCajaDto>>> GetAll();
        Task<ApiResponse<CorteCajaDto>> GetById(int id);
        Task<ApiResponse<CorteCajaDto>> GetCorteActivo(int idUsuario);
        Task<ApiResponse<IEnumerable<CorteCajaDto>>> GetByUsuario(int idUsuario);
        Task<ApiResponse<IEnumerable<CorteCajaDto>>> GetByFecha(DateTime fecha);
        Task<ApiResponse<CorteCajaDto>> AbrirCaja(AbrirCajaRequest request, int idUsuario);
        Task<ApiResponse<CorteCajaDto>> CerrarCaja(int idCorte, CerrarCajaRequest request);
    }
}