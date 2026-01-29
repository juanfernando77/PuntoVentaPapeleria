using PapeleriaAPI.DTOs;

namespace PapeleriaAPI.Services
{
    public interface IProductoService
    {
        Task<ApiResponse<IEnumerable<ProductoDto>>> GetAll();
        Task<ApiResponse<IEnumerable<ProductoDto>>> GetAllActivos();
        Task<ApiResponse<IEnumerable<ProductoDto>>> GetByCategoria(int idCategoria);
        Task<ApiResponse<IEnumerable<ProductoDto>>> GetProductosBajoStock();
        Task<ApiResponse<ProductoDto>> GetById(int id);
        Task<ApiResponse<ProductoDto>> GetByCodigo(string codigo);
        Task<ApiResponse<ProductoDto>> Create(CreateProductoRequest request);
        Task<ApiResponse<ProductoDto>> Update(int id, UpdateProductoRequest request);
        Task<ApiResponse<bool>> Delete(int id);
    }
}