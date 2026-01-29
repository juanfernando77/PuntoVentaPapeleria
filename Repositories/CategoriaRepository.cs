using Microsoft.EntityFrameworkCore;
using PapeleriaAPI.Data;
using PapeleriaAPI.Models;

namespace PapeleriaAPI.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoriaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Categoria>> GetAll()
        {
            return await _context.Categorias
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        public async Task<IEnumerable<Categoria>> GetAllActivas()
        {
            return await _context.Categorias
                .Where(c => c.Activo)
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        public async Task<Categoria?> GetById(int id)
        {
            return await _context.Categorias
                .FirstOrDefaultAsync(c => c.IdCategoria == id);
        }

        public async Task<Categoria> Create(Categoria categoria)
        {
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
            return categoria;
        }

        public async Task<Categoria> Update(Categoria categoria)
        {
            _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();
            return categoria;
        }

        public async Task<bool> Delete(int id)
        {
            var categoria = await GetById(id);
            if (categoria == null)
                return false;

            // Eliminación lógica (cambiar Activo a false)
            categoria.Activo = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.Categorias
                .AnyAsync(c => c.IdCategoria == id);
        }

        public async Task<bool> ExistsByNombre(string nombre, int? excludeId = null)
        {
            var query = _context.Categorias
                .Where(c => c.Nombre.ToLower() == nombre.ToLower());

            if (excludeId.HasValue)
            {
                query = query.Where(c => c.IdCategoria != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}