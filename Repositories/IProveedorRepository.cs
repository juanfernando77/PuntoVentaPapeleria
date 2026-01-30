using PapeleriaAPI.Models;

namespace PapeleriaAPI.Repositories
{
    public interface IProveedorRepository
    {
        Task<IEnumerable<Proveedor>> GetAll();
        Task<IEnumerable<Proveedor>> GetAllActivos();
        Task<Proveedor?> GetById(int id);
        Task<Proveedor?> GetByRFC(string rfc);
        Task<Proveedor> Create(Proveedor proveedor);
        Task<Proveedor> Update(Proveedor proveedor);
        Task<bool> Delete(int id);
        Task<bool> Exists(int id);
        Task<bool> ExistsByRFC(string rfc, int? excludeId = null);
    }
}