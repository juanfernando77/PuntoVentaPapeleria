using Microsoft.EntityFrameworkCore;
using PapeleriaAPI.Data;
using PapeleriaAPI.Models;

namespace PapeleriaAPI.Repositories
{
    public class ProveedorRepository : IProveedorRepository
    {
        private readonly ApplicationDbContext _context;

        public ProveedorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Proveedor>> GetAll()
        {
            return await _context.Proveedores
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        public async Task<IEnumerable<Proveedor>> GetAllActivos()
        {
            return await _context.Proveedores
                .Where(p => p.Activo)
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        public async Task<Proveedor?> GetById(int id)
        {
            return await _context.Proveedores
                .FirstOrDefaultAsync(p => p.IdProveedor == id);
        }

        public async Task<Proveedor?> GetByRFC(string rfc)
        {
            return await _context.Proveedores
                .FirstOrDefaultAsync(p => p.RFC == rfc);
        }

        public async Task<Proveedor> Create(Proveedor proveedor)
        {
            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();
            return proveedor;
        }

        public async Task<Proveedor> Update(Proveedor proveedor)
        {
            _context.Proveedores.Update(proveedor);
            await _context.SaveChangesAsync();
            return proveedor;
        }

        public async Task<bool> Delete(int id)
        {
            var proveedor = await GetById(id);
            if (proveedor == null)
                return false;

            proveedor.Activo = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.Proveedores
                .AnyAsync(p => p.IdProveedor == id);
        }

        public async Task<bool> ExistsByRFC(string rfc, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(rfc))
                return false;

            var query = _context.Proveedores
                .Where(p => p.RFC != null && p.RFC.ToUpper() == rfc.ToUpper());

            if (excludeId.HasValue)
            {
                query = query.Where(p => p.IdProveedor != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}