using PapeleriaAPI.DTOs;

namespace PapeleriaAPI.Services
{
    public interface ICategoriaService
    {
        Task<ApiResponse<IEnumerable<CategoriaDto>>> GetAll();
        Task<ApiResponse<IEnumerable<CategoriaDto>>> GetAllActivas();
        Task<ApiResponse<CategoriaDto>> GetById(int id);
        Task<ApiResponse<CategoriaDto>> Create(CreateCategoriaRequest request);
        Task<ApiResponse<CategoriaDto>> Update(int id, UpdateCategoriaRequest request);
        Task<ApiResponse<bool>> Delete(int id);
    }
}