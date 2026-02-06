using PapeleriaAPI.Models;

namespace PapeleriaAPI.Repositories
{
    public interface ICorteCajaRepository
    {
        Task<IEnumerable<CorteCaja>> GetAll();
        Task<CorteCaja?> GetById(int id);
        Task<CorteCaja?> GetCorteActivo(int idUsuario);
        Task<CorteCaja?> GetUltimoCorte(int idUsuario);
        Task<IEnumerable<CorteCaja>> GetByUsuario(int idUsuario);
        Task<IEnumerable<CorteCaja>> GetByFecha(DateTime fecha);
        Task<CorteCaja> AbrirCaja(CorteCaja corte);
        Task<CorteCaja> CerrarCaja(int idCorte, decimal montoFinal, string? observaciones);
        Task<bool> TieneCajaAbierta(int idUsuario);
    }
}