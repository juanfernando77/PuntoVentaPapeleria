using Microsoft.EntityFrameworkCore;
using PapeleriaAPI.Data;
using PapeleriaAPI.Models;

namespace PapeleriaAPI.Repositories
{
    public class CorteCajaRepository : ICorteCajaRepository
    {
        private readonly ApplicationDbContext _context;

        public CorteCajaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CorteCaja>> GetAll()
        {
            return await _context.CortesCaja
                .Include(c => c.Usuario)
                .OrderByDescending(c => c.FechaApertura)
                .ToListAsync();
        }

        public async Task<CorteCaja?> GetById(int id)
        {
            return await _context.CortesCaja
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.IdCorte == id);
        }

        public async Task<CorteCaja?> GetCorteActivo(int idUsuario)
        {
            return await _context.CortesCaja
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.IdUsuario == idUsuario && !c.Cerrado);
        }

        public async Task<CorteCaja?> GetUltimoCorte(int idUsuario)
        {
            return await _context.CortesCaja
                .Include(c => c.Usuario)
                .Where(c => c.IdUsuario == idUsuario)
                .OrderByDescending(c => c.FechaApertura)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CorteCaja>> GetByUsuario(int idUsuario)
        {
            return await _context.CortesCaja
                .Include(c => c.Usuario)
                .Where(c => c.IdUsuario == idUsuario)
                .OrderByDescending(c => c.FechaApertura)
                .ToListAsync();
        }

        public async Task<IEnumerable<CorteCaja>> GetByFecha(DateTime fecha)
        {
            var fechaInicio = fecha.Date;
            var fechaFin = fecha.Date.AddDays(1);

            return await _context.CortesCaja
                .Include(c => c.Usuario)
                .Where(c => c.FechaApertura >= fechaInicio && c.FechaApertura < fechaFin)
                .OrderByDescending(c => c.FechaApertura)
                .ToListAsync();
        }

        public async Task<CorteCaja> AbrirCaja(CorteCaja corte)
        {
            _context.CortesCaja.Add(corte);
            await _context.SaveChangesAsync();

            return (await GetById(corte.IdCorte))!;
        }

        public async Task<CorteCaja> CerrarCaja(int idCorte, decimal montoFinal, string? observaciones)
        {
            var corte = await GetById(idCorte);
            if (corte == null)
                throw new InvalidOperationException("Corte de caja no encontrado");

            var fechaApertura = corte.FechaApertura;
            var fechaCierre = DateTime.Now;

            var ventasDelPeriodo = await _context.Ventas
                .Where(v => v.FechaVenta >= fechaApertura && v.FechaVenta <= fechaCierre)
                .ToListAsync();

            corte.VentasEfectivo = ventasDelPeriodo.Where(v => v.MetodoPago == "Efectivo").Sum(v => v.Total);
            corte.VentasTarjeta = ventasDelPeriodo.Where(v => v.MetodoPago == "Tarjeta").Sum(v => v.Total);
            corte.VentasTransferencia = ventasDelPeriodo.Where(v => v.MetodoPago == "Transferencia").Sum(v => v.Total);
            corte.TotalVentas = ventasDelPeriodo.Sum(v => v.Total);
            corte.MontoFinal = montoFinal;
            corte.Diferencia = montoFinal - (corte.MontoInicial + corte.VentasEfectivo);
            corte.FechaCierre = fechaCierre;
            corte.Observaciones = observaciones;
            corte.Cerrado = true;

            await _context.SaveChangesAsync();

            return (await GetById(idCorte))!;
        }

        public async Task<bool> TieneCajaAbierta(int idUsuario)
        {
            return await _context.CortesCaja
                .AnyAsync(c => c.IdUsuario == idUsuario && !c.Cerrado);
        }
    }
}