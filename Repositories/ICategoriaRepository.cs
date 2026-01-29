using PapeleriaAPI.Models;

namespace PapeleriaAPI.Repositories
{
    public interface ICategoriaRepository
    {
        Task<IEnumerable<Categoria>> GetAll();
        Task<IEnumerable<Categoria>> GetAllActivas();
        Task<Categoria?> GetById(int id);
        Task<Categoria> Create(Categoria categoria);
        Task<Categoria> Update(Categoria categoria);
        Task<bool> Delete(int id);
        Task<bool> Exists(int id);
        Task<bool> ExistsByNombre(string nombre, int? excludeId = null);
    }
}