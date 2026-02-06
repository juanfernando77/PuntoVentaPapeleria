using PapeleriaAPI.DTOs;
using PapeleriaAPI.Models;
using PapeleriaAPI.Repositories;

namespace PapeleriaAPI.Services
{
    public class CorteCajaService : ICorteCajaService
    {
        private readonly ICorteCajaRepository _corteCajaRepository;

        public CorteCajaService(ICorteCajaRepository corteCajaRepository)
        {
            _corteCajaRepository = corteCajaRepository;
        }

        public async Task<ApiResponse<IEnumerable<CorteCajaDto>>> GetAll()
        {
            try
            {
                var cortes = await _corteCajaRepository.GetAll();
                var cortesDto = cortes.Select(c => MapToDto(c));

                return new ApiResponse<IEnumerable<CorteCajaDto>>
                {
                    Success = true,
                    Message = "Cortes de caja obtenidos exitosamente",
                    Data = cortesDto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<CorteCajaDto>>
                {
                    Success = false,
                    Message = $"Error al obtener cortes de caja: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<CorteCajaDto>> GetById(int id)
        {
            try
            {
                var corte = await _corteCajaRepository.GetById(id);

                if (corte == null)
                {
                    return new ApiResponse<CorteCajaDto>
                    {
                        Success = false,
                        Message = "Corte de caja no encontrado"
                    };
                }

                return new ApiResponse<CorteCajaDto>
                {
                    Success = true,
                    Message = "Corte de caja obtenido exitosamente",
                    Data = MapToDto(corte)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<CorteCajaDto>
                {
                    Success = false,
                    Message = $"Error al obtener corte de caja: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<CorteCajaDto>> GetCorteActivo(int idUsuario)
        {
            try
            {
                var corte = await _corteCajaRepository.GetCorteActivo(idUsuario);

                if (corte == null)
                {
                    return new ApiResponse<CorteCajaDto>
                    {
                        Success = false,
                        Message = "No hay caja abierta actualmente"
                    };
                }

                return new ApiResponse<CorteCajaDto>
                {
                    Success = true,
                    Message = "Corte activo obtenido exitosamente",
                    Data = MapToDto(corte)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<CorteCajaDto>
                {
                    Success = false,
                    Message = $"Error al obtener corte activo: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<IEnumerable<CorteCajaDto>>> GetByUsuario(int idUsuario)
        {
            try
            {
                var cortes = await _corteCajaRepository.GetByUsuario(idUsuario);
                var cortesDto = cortes.Select(c => MapToDto(c));

                return new ApiResponse<IEnumerable<CorteCajaDto>>
                {
                    Success = true,
                    Message = "Cortes del usuario obtenidos exitosamente",
                    Data = cortesDto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<CorteCajaDto>>
                {
                    Success = false,
                    Message = $"Error al obtener cortes del usuario: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<IEnumerable<CorteCajaDto>>> GetByFecha(DateTime fecha)
        {
            try
            {
                var cortes = await _corteCajaRepository.GetByFecha(fecha);
                var cortesDto = cortes.Select(c => MapToDto(c));

                return new ApiResponse<IEnumerable<CorteCajaDto>>
                {
                    Success = true,
                    Message = $"Cortes del {fecha:dd/MM/yyyy} obtenidos exitosamente",
                    Data = cortesDto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<CorteCajaDto>>
                {
                    Success = false,
                    Message = $"Error al obtener cortes por fecha: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<CorteCajaDto>> AbrirCaja(AbrirCajaRequest request, int idUsuario)
        {
            try
            {
                if (await _corteCajaRepository.TieneCajaAbierta(idUsuario))
                {
                    return new ApiResponse<CorteCajaDto>
                    {
                        Success = false,
                        Message = "Ya tienes una caja abierta. Debes cerrarla antes de abrir una nueva."
                    };
                }

                var corte = new CorteCaja
                {
                    IdUsuario = idUsuario,
                    FechaApertura = DateTime.Now,
                    MontoInicial = request.MontoInicial,
                    Cerrado = false
                };

                var corteCreado = await _corteCajaRepository.AbrirCaja(corte);

                return new ApiResponse<CorteCajaDto>
                {
                    Success = true,
                    Message = "Caja abierta exitosamente",
                    Data = MapToDto(corteCreado)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<CorteCajaDto>
                {
                    Success = false,
                    Message = $"Error al abrir caja: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<CorteCajaDto>> CerrarCaja(int idCorte, CerrarCajaRequest request)
        {
            try
            {
                var corte = await _corteCajaRepository.GetById(idCorte);

                if (corte == null)
                {
                    return new ApiResponse<CorteCajaDto>
                    {
                        Success = false,
                        Message = "Corte de caja no encontrado"
                    };
                }

                if (corte.Cerrado)
                {
                    return new ApiResponse<CorteCajaDto>
                    {
                        Success = false,
                        Message = "Esta caja ya está cerrada"
                    };
                }

                var corteCerrado = await _corteCajaRepository.CerrarCaja(
                    idCorte,
                    request.MontoFinal,
                    request.Observaciones
                );

                return new ApiResponse<CorteCajaDto>
                {
                    Success = true,
                    Message = "Caja cerrada exitosamente",
                    Data = MapToDto(corteCerrado)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<CorteCajaDto>
                {
                    Success = false,
                    Message = $"Error al cerrar caja: {ex.Message}"
                };
            }
        }

        private CorteCajaDto MapToDto(CorteCaja corte)
        {
            var dto = new CorteCajaDto
            {
                IdCorte = corte.IdCorte,
                IdUsuario = corte.IdUsuario,
                NombreUsuario = corte.Usuario?.NombreUsuario ?? "",
                FechaApertura = corte.FechaApertura,
                FechaCierre = corte.FechaCierre,
                MontoInicial = corte.MontoInicial,
                VentasEfectivo = corte.VentasEfectivo,
                VentasTarjeta = corte.VentasTarjeta,
                VentasTransferencia = corte.VentasTransferencia,
                TotalVentas = corte.TotalVentas,
                MontoFinal = corte.MontoFinal,
                Diferencia = corte.Diferencia,
                Observaciones = corte.Observaciones,
                Cerrado = corte.Cerrado
            };

            if (corte.FechaCierre.HasValue)
            {
                dto.TiempoAbierto = corte.FechaCierre.Value - corte.FechaApertura;
            }

            return dto;
        }
    }
}