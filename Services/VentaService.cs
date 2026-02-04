using PapeleriaAPI.DTOs;
using PapeleriaAPI.Models;
using PapeleriaAPI.Repositories;

namespace PapeleriaAPI.Services
{
    public class VentaService : IVentaService
    {
        private readonly IVentaRepository _ventaRepository;
        private readonly IProductoRepository _productoRepository;

        public VentaService(
            IVentaRepository ventaRepository,
            IProductoRepository productoRepository)
        {
            _ventaRepository = ventaRepository;
            _productoRepository = productoRepository;
        }

        public async Task<ApiResponse<IEnumerable<VentaResumenDto>>> GetAll()
        {
            try
            {
                var ventas = await _ventaRepository.GetAll();
                var ventasDto = ventas.Select(v => MapToResumenDto(v));

                return new ApiResponse<IEnumerable<VentaResumenDto>>
                {
                    Success = true,
                    Message = "Ventas obtenidas exitosamente",
                    Data = ventasDto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<VentaResumenDto>>
                {
                    Success = false,
                    Message = $"Error al obtener ventas: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<IEnumerable<VentaResumenDto>>> GetVentasHoy()
        {
            try
            {
                var ventas = await _ventaRepository.GetVentasHoy();
                var ventasDto = ventas.Select(v => MapToResumenDto(v));

                return new ApiResponse<IEnumerable<VentaResumenDto>>
                {
                    Success = true,
                    Message = "Ventas del día obtenidas exitosamente",
                    Data = ventasDto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<VentaResumenDto>>
                {
                    Success = false,
                    Message = $"Error al obtener ventas del día: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<IEnumerable<VentaResumenDto>>> GetByFecha(DateTime fecha)
        {
            try
            {
                var ventas = await _ventaRepository.GetByFecha(fecha);
                var ventasDto = ventas.Select(v => MapToResumenDto(v));

                return new ApiResponse<IEnumerable<VentaResumenDto>>
                {
                    Success = true,
                    Message = $"Ventas del {fecha:dd/MM/yyyy} obtenidas exitosamente",
                    Data = ventasDto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<VentaResumenDto>>
                {
                    Success = false,
                    Message = $"Error al obtener ventas por fecha: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<IEnumerable<VentaResumenDto>>> GetByPeriodo(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                if (fechaInicio > fechaFin)
                {
                    return new ApiResponse<IEnumerable<VentaResumenDto>>
                    {
                        Success = false,
                        Message = "La fecha de inicio no puede ser mayor a la fecha fin"
                    };
                }

                var ventas = await _ventaRepository.GetByPeriodo(fechaInicio, fechaFin);
                var ventasDto = ventas.Select(v => MapToResumenDto(v));

                return new ApiResponse<IEnumerable<VentaResumenDto>>
                {
                    Success = true,
                    Message = $"Ventas del período obtenidas exitosamente",
                    Data = ventasDto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<VentaResumenDto>>
                {
                    Success = false,
                    Message = $"Error al obtener ventas por período: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<VentaDto>> GetById(int id)
        {
            try
            {
                var venta = await _ventaRepository.GetById(id);

                if (venta == null)
                {
                    return new ApiResponse<VentaDto>
                    {
                        Success = false,
                        Message = "Venta no encontrada"
                    };
                }

                return new ApiResponse<VentaDto>
                {
                    Success = true,
                    Message = "Venta obtenida exitosamente",
                    Data = MapToDto(venta)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<VentaDto>
                {
                    Success = false,
                    Message = $"Error al obtener venta: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<VentaDto>> Create(CreateVentaRequest request, int idUsuario)
        {
            try
            {
                // 1. Validar que haya items
                if (request.Items == null || !request.Items.Any())
                {
                    return new ApiResponse<VentaDto>
                    {
                        Success = false,
                        Message = "Debe agregar al menos un producto a la venta"
                    };
                }

                // 2. Validar stock disponible para todos los productos
                foreach (var item in request.Items)
                {
                    var producto = await _productoRepository.GetById(item.IdProducto);

                    if (producto == null)
                    {
                        return new ApiResponse<VentaDto>
                        {
                            Success = false,
                            Message = $"El producto con ID {item.IdProducto} no existe"
                        };
                    }

                    if (!producto.Activo)
                    {
                        return new ApiResponse<VentaDto>
                        {
                            Success = false,
                            Message = $"El producto '{producto.Nombre}' no está activo"
                        };
                    }

                    if (producto.Stock < item.Cantidad)
                    {
                        return new ApiResponse<VentaDto>
                        {
                            Success = false,
                            Message = $"Stock insuficiente para '{producto.Nombre}'. Disponible: {producto.Stock}, Solicitado: {item.Cantidad}"
                        };
                    }
                }

                // 3. Calcular totales
                decimal subtotal = request.Items.Sum(i => i.Cantidad * i.PrecioVenta);
                decimal iva = request.AplicarIVA ? subtotal * 0.16m : 0;
                decimal total = subtotal + iva;

                // 4. Validar monto pagado
                if (request.MetodoPago == "Efectivo" && request.MontoPagado < total)
                {
                    return new ApiResponse<VentaDto>
                    {
                        Success = false,
                        Message = $"El monto pagado (${request.MontoPagado:N2}) es menor al total (${total:N2})"
                    };
                }

                decimal cambio = request.MetodoPago == "Efectivo" ? request.MontoPagado - total : 0;

                // 5. Generar número de venta
                var numeroVenta = await _ventaRepository.GenerarNumeroVenta();

                // 6. Crear venta
                var venta = new Venta
                {
                    NumeroVenta = numeroVenta,
                    IdUsuario = idUsuario,
                    FechaVenta = DateTime.Now,
                    Subtotal = subtotal,
                    IVA = iva,
                    Total = total,
                    MetodoPago = request.MetodoPago,
                    MontoPagado = request.MontoPagado,
                    Cambio = cambio
                };

                // 7. Crear detalles
                var detalles = request.Items.Select(i => new DetalleVenta
                {
                    IdProducto = i.IdProducto,
                    Cantidad = i.Cantidad,
                    PrecioVenta = i.PrecioVenta,
                    Subtotal = i.Cantidad * i.PrecioVenta
                }).ToList();

                // 8. Guardar en la base de datos (con transacción)
                var ventaCreada = await _ventaRepository.Create(venta, detalles);

                return new ApiResponse<VentaDto>
                {
                    Success = true,
                    Message = $"Venta {numeroVenta} registrada exitosamente",
                    Data = MapToDto(ventaCreada)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<VentaDto>
                {
                    Success = false,
                    Message = $"Error al crear venta: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<bool>> CancelarVenta(int id)
        {
            try
            {
                var venta = await _ventaRepository.GetById(id);

                if (venta == null)
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Venta no encontrada"
                    };
                }

                // Validar que la venta no sea muy antigua (opcional)
                var diasMaximos = 7;
                if ((DateTime.Now - venta.FechaVenta).TotalDays > diasMaximos)
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = $"No se pueden cancelar ventas con más de {diasMaximos} días de antigüedad"
                    };
                }

                var cancelada = await _ventaRepository.CancelarVenta(id);

                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = $"Venta {venta.NumeroVenta} cancelada exitosamente. Stock devuelto.",
                    Data = cancelada
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Error al cancelar venta: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<Dictionary<string, object>>> GetEstadisticasHoy()
        {
            try
            {
                var fecha = DateTime.Now.Date;
                var totalVentas = await _ventaRepository.GetTotalVentasPorFecha(fecha);
                var cantidadVentas = await _ventaRepository.GetCantidadVentasPorFecha(fecha);

                var estadisticas = new Dictionary<string, object>
                {
                    { "fecha", fecha.ToString("dd/MM/yyyy") },
                    { "totalVentas", totalVentas },
                    { "cantidadVentas", cantidadVentas },
                    { "promedioVenta", cantidadVentas > 0 ? totalVentas / cantidadVentas : 0 }
                };

                return new ApiResponse<Dictionary<string, object>>
                {
                    Success = true,
                    Message = "Estadísticas obtenidas exitosamente",
                    Data = estadisticas
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<Dictionary<string, object>>
                {
                    Success = false,
                    Message = $"Error al obtener estadísticas: {ex.Message}"
                };
            }
        }

        // Métodos auxiliares de mapeo
        private VentaDto MapToDto(Venta venta)
        {
            return new VentaDto
            {
                IdVenta = venta.IdVenta,
                NumeroVenta = venta.NumeroVenta,
                IdUsuario = venta.IdUsuario,
                NombreUsuario = venta.Usuario?.NombreUsuario ?? "",
                FechaVenta = venta.FechaVenta,
                Subtotal = venta.Subtotal,
                IVA = venta.IVA,
                Total = venta.Total,
                MetodoPago = venta.MetodoPago,
                MontoPagado = venta.MontoPagado,
                Cambio = venta.Cambio,
                CantidadProductos = venta.DetallesVenta?.Sum(d => d.Cantidad) ?? 0,
                Detalles = venta.DetallesVenta?.Select(d => new DetalleVentaDto
                {
                    IdDetalleVenta = d.IdDetalleVenta,
                    IdProducto = d.IdProducto,
                    CodigoProducto = d.Producto?.Codigo ?? "",
                    NombreProducto = d.Producto?.Nombre ?? "",
                    Cantidad = d.Cantidad,
                    PrecioVenta = d.PrecioVenta,
                    Subtotal = d.Subtotal
                }).ToList() ?? new List<DetalleVentaDto>()
            };
        }

        private VentaResumenDto MapToResumenDto(Venta venta)
        {
            return new VentaResumenDto
            {
                IdVenta = venta.IdVenta,
                NumeroVenta = venta.NumeroVenta,
                NombreUsuario = venta.Usuario?.NombreUsuario ?? "",
                FechaVenta = venta.FechaVenta,
                Total = venta.Total,
                MetodoPago = venta.MetodoPago,
                CantidadProductos = venta.DetallesVenta?.Sum(d => d.Cantidad) ?? 0
            };
        }
    }
}