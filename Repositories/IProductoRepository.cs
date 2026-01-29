using PapeleriaAPI.Models;

namespace PapeleriaAPI.Repositories
{
    public interface IProductoRepository
    {
        Task<IEnumerable<Producto>> GetAll();
        Task<IEnumerable<Producto>> GetAllActivos();
        Task<IEnumerable<Producto>> GetByCategoria(int idCategoria);
        Task<IEnumerable<Producto>> GetProductosBajoStock();
        Task<Producto?> GetById(int id);
        Task<Producto?> GetByCodigo(string codigo);
        Task<Producto> Create(Producto producto);
        Task<Producto> Update(Producto producto);
        Task<bool> Delete(int id);
        Task<bool> Exists(int id);
        Task<bool> ExistsByCodigo(string codigo, int? excludeId = null);
        Task<bool> ActualizarStock(int id, int cantidad, bool esCompra);
    }
}