using Microsoft.EntityFrameworkCore;
using PapeleriaAPI.Data;
using PapeleriaAPI.Models;

namespace PapeleriaAPI.Repositories
{
    public class CompraRepository : ICompraRepository
    {
        private readonly ApplicationDbContext _context;

        public CompraRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Compra>> GetAll()
        {
            return await _context.Compras
                .Include(c => c.Proveedor)
                .Include(c => c.Usuario)
                .Include(c => c.DetallesCompra)
                    .ThenInclude(d => d.Producto)
                .OrderByDescending(c => c.FechaCompra)
                .ToListAsync();
        }

        public async Task<IEnumerable<Compra>> GetByFecha(DateTime fecha)
        {
            var fechaInicio = fecha.Date;
            var fechaFin = fecha.Date.AddDays(1);

            return await _context.Compras
                .Include(c => c.Proveedor)
                .Include(c => c.Usuario)
                .Include(c => c.DetallesCompra)
                    .ThenInclude(d => d.Producto)
                .Where(c => c.FechaCompra >= fechaInicio && c.FechaCompra < fechaFin)
                .OrderByDescending(c => c.FechaCompra)
                .ToListAsync();
        }

        public async Task<IEnumerable<Compra>> GetByProveedor(int idProveedor)
        {
            return await _context.Compras
                .Include(c => c.Proveedor)
                .Include(c => c.Usuario)
                .Include(c => c.DetallesCompra)
                    .ThenInclude(d => d.Producto)
                .Where(c => c.IdProveedor == idProveedor)
                .OrderByDescending(c => c.FechaCompra)
                .ToListAsync();
        }

        public async Task<IEnumerable<Compra>> GetByPeriodo(DateTime fechaInicio, DateTime fechaFin)
        {
            var inicio = fechaInicio.Date;
            var fin = fechaFin.Date.AddDays(1);

            return await _context.Compras
                .Include(c => c.Proveedor)
                .Include(c => c.Usuario)
                .Include(c => c.DetallesCompra)
                    .ThenInclude(d => d.Producto)
                .Where(c => c.FechaCompra >= inicio && c.FechaCompra < fin)
                .OrderByDescending(c => c.FechaCompra)
                .ToListAsync();
        }

        public async Task<Compra?> GetById(int id)
        {
            return await _context.Compras
                .Include(c => c.Proveedor)
                .Include(c => c.Usuario)
                .Include(c => c.DetallesCompra)
                    .ThenInclude(d => d.Producto)
                        .ThenInclude(p => p.Categoria)
                .FirstOrDefaultAsync(c => c.IdCompra == id);
        }

        public async Task<Compra> Create(Compra compra, List<DetalleCompra> detalles)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Compras.Add(compra);
                await _context.SaveChangesAsync();

                foreach (var detalle in detalles)
                {
                    detalle.IdCompra = compra.IdCompra;
                    _context.DetallesCompra.Add(detalle);

                    var producto = await _context.Productos.FindAsync(detalle.IdProducto);
                    if (producto != null)
                    {
                        producto.Stock += detalle.Cantidad;
                        producto.PrecioCompra = detalle.PrecioCompra;
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (await GetById(compra.IdCompra))!;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> CancelarCompra(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var compra = await GetById(id);
                if (compra == null)
                    return false;

                foreach (var detalle in compra.DetallesCompra ?? new List<DetalleCompra>())
                {
                    var producto = await _context.Productos.FindAsync(detalle.IdProducto);
                    if (producto != null)
                    {
                        producto.Stock -= detalle.Cantidad;

                        if (producto.Stock < 0)
                        {
                            await transaction.RollbackAsync();
                            throw new InvalidOperationException(
                                $"No se puede cancelar la compra. El producto '{producto.Nombre}' no tiene suficiente stock para devolver.");
                        }
                    }
                }

                _context.Compras.Remove(compra);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<string> GenerarNumeroCompra()
        {
            var fecha = DateTime.Now;
            var prefijo = $"C-{fecha:yyyyMMdd}";

            var ultimaCompra = await _context.Compras
                .Where(c => c.NumeroCompra.StartsWith(prefijo))
                .OrderByDescending(c => c.NumeroCompra)
                .FirstOrDefaultAsync();

            if (ultimaCompra == null)
            {
                return $"{prefijo}-0001";
            }

            var ultimoNumero = int.Parse(ultimaCompra.NumeroCompra.Split('-').Last());
            var nuevoNumero = ultimoNumero + 1;

            return $"{prefijo}-{nuevoNumero:D4}";
        }

        public async Task<decimal> GetTotalComprasPorFecha(DateTime fecha)
        {
            var fechaInicio = fecha.Date;
            var fechaFin = fecha.Date.AddDays(1);

            return await _context.Compras
                .Where(c => c.FechaCompra >= fechaInicio && c.FechaCompra < fechaFin)
                .SumAsync(c => c.Total);
        }

        public async Task<int> GetCantidadComprasPorFecha(DateTime fecha)
        {
            var fechaInicio = fecha.Date;
            var fechaFin = fecha.Date.AddDays(1);

            return await _context.Compras
                .Where(c => c.FechaCompra >= fechaInicio && c.FechaCompra < fechaFin)
                .CountAsync();
        }
    }
}