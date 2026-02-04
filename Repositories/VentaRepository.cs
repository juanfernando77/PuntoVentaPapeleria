using Microsoft.EntityFrameworkCore;
using PapeleriaAPI.Data;
using PapeleriaAPI.Models;

namespace PapeleriaAPI.Repositories
{
    public class VentaRepository : IVentaRepository
    {
        private readonly ApplicationDbContext _context;

        public VentaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Venta>> GetAll()
        {
            return await _context.Ventas
                .Include(v => v.Usuario)
                .Include(v => v.DetallesVenta)
                    .ThenInclude(d => d.Producto)
                .OrderByDescending(v => v.FechaVenta)
                .ToListAsync();
        }

        public async Task<IEnumerable<Venta>> GetByFecha(DateTime fecha)
        {
            var fechaInicio = fecha.Date;
            var fechaFin = fecha.Date.AddDays(1);

            return await _context.Ventas
                .Include(v => v.Usuario)
                .Include(v => v.DetallesVenta)
                    .ThenInclude(d => d.Producto)
                .Where(v => v.FechaVenta >= fechaInicio && v.FechaVenta < fechaFin)
                .OrderByDescending(v => v.FechaVenta)
                .ToListAsync();
        }

        public async Task<IEnumerable<Venta>> GetByPeriodo(DateTime fechaInicio, DateTime fechaFin)
        {
            var inicio = fechaInicio.Date;
            var fin = fechaFin.Date.AddDays(1);

            return await _context.Ventas
                .Include(v => v.Usuario)
                .Include(v => v.DetallesVenta)
                    .ThenInclude(d => d.Producto)
                .Where(v => v.FechaVenta >= inicio && v.FechaVenta < fin)
                .OrderByDescending(v => v.FechaVenta)
                .ToListAsync();
        }

        public async Task<IEnumerable<Venta>> GetVentasHoy()
        {
            return await GetByFecha(DateTime.Now.Date);
        }

        public async Task<Venta?> GetById(int id)
        {
            return await _context.Ventas
                .Include(v => v.Usuario)
                .Include(v => v.DetallesVenta)
                    .ThenInclude(d => d.Producto)
                        .ThenInclude(p => p.Categoria)
                .FirstOrDefaultAsync(v => v.IdVenta == id);
        }

        public async Task<Venta> Create(Venta venta, List<DetalleVenta> detalles)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Crear la venta
                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync();

                // 2. Agregar los detalles y actualizar stock
                foreach (var detalle in detalles)
                {
                    detalle.IdVenta = venta.IdVenta;
                    _context.DetallesVenta.Add(detalle);

                    // 3. Actualizar stock del producto
                    var producto = await _context.Productos.FindAsync(detalle.IdProducto);
                    if (producto != null)
                    {
                        producto.Stock -= detalle.Cantidad;
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // 4. Recargar la venta con sus relaciones
                return (await GetById(venta.IdVenta))!;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> CancelarVenta(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var venta = await GetById(id);
                if (venta == null)
                    return false;

                // Devolver el stock de los productos
                foreach (var detalle in venta.DetallesVenta ?? new List<DetalleVenta>())
                {
                    var producto = await _context.Productos.FindAsync(detalle.IdProducto);
                    if (producto != null)
                    {
                        producto.Stock += detalle.Cantidad;
                    }
                }

                // Eliminar la venta
                _context.Ventas.Remove(venta);
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

        public async Task<string> GenerarNumeroVenta()
        {
            var fecha = DateTime.Now;
            var prefijo = $"V-{fecha:yyyyMMdd}";

            var ultimaVenta = await _context.Ventas
                .Where(v => v.NumeroVenta.StartsWith(prefijo))
                .OrderByDescending(v => v.NumeroVenta)
                .FirstOrDefaultAsync();

            if (ultimaVenta == null)
            {
                return $"{prefijo}-0001";
            }

            var ultimoNumero = int.Parse(ultimaVenta.NumeroVenta.Split('-').Last());
            var nuevoNumero = ultimoNumero + 1;

            return $"{prefijo}-{nuevoNumero:D4}";
        }

        public async Task<decimal> GetTotalVentasPorFecha(DateTime fecha)
        {
            var fechaInicio = fecha.Date;
            var fechaFin = fecha.Date.AddDays(1);

            return await _context.Ventas
                .Where(v => v.FechaVenta >= fechaInicio && v.FechaVenta < fechaFin)
                .SumAsync(v => v.Total);
        }

        public async Task<int> GetCantidadVentasPorFecha(DateTime fecha)
        {
            var fechaInicio = fecha.Date;
            var fechaFin = fecha.Date.AddDays(1);

            return await _context.Ventas
                .Where(v => v.FechaVenta >= fechaInicio && v.FechaVenta < fechaFin)
                .CountAsync();
        }
    }
}