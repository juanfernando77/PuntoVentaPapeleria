using PapeleriaAPI.DTOs;

namespace PapeleriaAPI.Services
{
    public interface IProveedorService
    {
        Task<ApiResponse<IEnumerable<ProveedorDto>>> GetAll();
        Task<ApiResponse<IEnumerable<ProveedorDto>>> GetAllActivos();
        Task<ApiResponse<ProveedorDto>> GetById(int id);
        Task<ApiResponse<ProveedorDto>> Create(CreateProveedorRequest request);
        Task<ApiResponse<ProveedorDto>> Update(int id, UpdateProveedorRequest request);
        Task<ApiResponse<bool>> Delete(int id);
    }
}