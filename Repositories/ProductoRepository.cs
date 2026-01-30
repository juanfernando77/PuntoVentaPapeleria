using Microsoft.EntityFrameworkCore;
using PapeleriaAPI.Data;
using PapeleriaAPI.Models;

namespace PapeleriaAPI.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Producto>> GetAll()
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        public async Task<IEnumerable<Producto>> GetAllActivos()
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.Activo)
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        public async Task<IEnumerable<Producto>> GetByCategoria(int idCategoria)
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.IdCategoria == idCategoria && p.Activo)
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        public async Task<IEnumerable<Producto>> GetProductosBajoStock()
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.Activo && p.Stock <= p.StockMinimo)
                .OrderBy(p => p.Stock)
                .ToListAsync();
        }

        public async Task<Producto?> GetById(int id)
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.IdProducto == id);
        }

        public async Task<Producto?> GetByCodigo(string codigo)
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Codigo == codigo);
        }

        public async Task<Producto> Create(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            // Recargar con la categoría
            await _context.Entry(producto)
                .Reference(p => p.Categoria)
                .LoadAsync();

            return producto;
        }

        public async Task<Producto> Update(Producto producto)
        {
            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();

            // Recargar con la categoría
            await _context.Entry(producto)
                .Reference(p => p.Categoria)
                .LoadAsync();

            return producto;
        }

        public async Task<bool> Delete(int id)
        {
            var producto = await GetById(id);
            if (producto == null)
                return false;

            // Eliminación lógica
            producto.Activo = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.Productos
                .AnyAsync(p => p.IdProducto == id);
        }

        public async Task<bool> ExistsByCodigo(string codigo, int? excludeId = null)
        {
            var query = _context.Productos
                .Where(p => p.Codigo.ToLower() == codigo.ToLower());

            if (excludeId.HasValue)
            {
                query = query.Where(p => p.IdProducto != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> ActualizarStock(int id, int cantidad, bool esCompra)
        {
            var producto = await GetById(id);
            if (producto == null)
                return false;

            if (esCompra)
            {
                
                producto.Stock += cantidad;
            }
            else
            {
               
                if (producto.Stock < cantidad)
                    return false; 

                producto.Stock -= cantidad;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}