using PapeleriaAPI.DTOs;
using PapeleriaAPI.Models;
using PapeleriaAPI.Repositories;

namespace PapeleriaAPI.Services
{
    public class CompraService : ICompraService
    {
        private readonly ICompraRepository _compraRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly IProveedorRepository _proveedorRepository;

        public CompraService(
            ICompraRepository compraRepository,
            IProductoRepository productoRepository,
            IProveedorRepository proveedorRepository)
        {
            _compraRepository = compraRepository;
            _productoRepository = productoRepository;
            _proveedorRepository = proveedorRepository;
        }

        public async Task<ApiResponse<IEnumerable<CompraResumenDto>>> GetAll()
        {
            try
            {
                var compras = await _compraRepository.GetAll();
                var comprasDto = compras.Select(c => MapToResumenDto(c));

                return new ApiResponse<IEnumerable<CompraResumenDto>>
                {
                    Success = true,
                    Message = "Compras obtenidas exitosamente",
                    Data = comprasDto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<CompraResumenDto>>
                {
                    Success = false,
                    Message = $"Error al obtener compras: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<IEnumerable<CompraResumenDto>>> GetByFecha(DateTime fecha)
        {
            try
            {
                var compras = await _compraRepository.GetByFecha(fecha);
                var comprasDto = compras.Select(c => MapToResumenDto(c));

                return new ApiResponse<IEnumerable<CompraResumenDto>>
                {
                    Success = true,
                    Message = $"Compras del {fecha:dd/MM/yyyy} obtenidas exitosamente",
                    Data = comprasDto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<CompraResumenDto>>
                {
                    Success = false,
                    Message = $"Error al obtener compras por fecha: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<IEnumerable<CompraResumenDto>>> GetByProveedor(int idProveedor)
        {
            try
            {
                if (!await _proveedorRepository.Exists(idProveedor))
                {
                    return new ApiResponse<IEnumerable<CompraResumenDto>>
                    {
                        Success = false,
                        Message = "Proveedor no encontrado"
                    };
                }

                var compras = await _compraRepository.GetByProveedor(idProveedor);
                var comprasDto = compras.Select(c => MapToResumenDto(c));

                return new ApiResponse<IEnumerable<CompraResumenDto>>
                {
                    Success = true,
                    Message = "Compras del proveedor obtenidas exitosamente",
                    Data = comprasDto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<CompraResumenDto>>
                {
                    Success = false,
                    Message = $"Error al obtener compras por proveedor: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<IEnumerable<CompraResumenDto>>> GetByPeriodo(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                if (fechaInicio > fechaFin)
                {
                    return new ApiResponse<IEnumerable<CompraResumenDto>>
                    {
                        Success = false,
                        Message = "La fecha de inicio no puede ser mayor a la fecha fin"
                    };
                }

                var compras = await _compraRepository.GetByPeriodo(fechaInicio, fechaFin);
                var comprasDto = compras.Select(c => MapToResumenDto(c));

                return new ApiResponse<IEnumerable<CompraResumenDto>>
                {
                    Success = true,
                    Message = "Compras del período obtenidas exitosamente",
                    Data = comprasDto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<CompraResumenDto>>
                {
                    Success = false,
                    Message = $"Error al obtener compras por período: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<CompraDto>> GetById(int id)
        {
            try
            {
                var compra = await _compraRepository.GetById(id);

                if (compra == null)
                {
                    return new ApiResponse<CompraDto>
                    {
                        Success = false,
                        Message = "Compra no encontrada"
                    };
                }

                return new ApiResponse<CompraDto>
                {
                    Success = true,
                    Message = "Compra obtenida exitosamente",
                    Data = MapToDto(compra)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<CompraDto>
                {
                    Success = false,
                    Message = $"Error al obtener compra: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<CompraDto>> Create(CreateCompraRequest request, int idUsuario)
        {
            try
            {
                if (request.Items == null || !request.Items.Any())
                {
                    return new ApiResponse<CompraDto>
                    {
                        Success = false,
                        Message = "Debe agregar al menos un producto a la compra"
                    };
                }

                if (!await _proveedorRepository.Exists(request.IdProveedor))
                {
                    return new ApiResponse<CompraDto>
                    {
                        Success = false,
                        Message = "El proveedor seleccionado no existe"
                    };
                }
                foreach (var item in request.Items)
                {
                    var producto = await _productoRepository.GetById(item.IdProducto);

                    if (producto == null)
                    {
                        return new ApiResponse<CompraDto>
                        {
                            Success = false,
                            Message = $"El producto con ID {item.IdProducto} no existe"
                        };
                    }

                    if (!producto.Activo)
                    {
                        return new ApiResponse<CompraDto>
                        {
                            Success = false,
                            Message = $"El producto '{producto.Nombre}' no está activo"
                        };
                    }
                }

                decimal total = request.Items.Sum(i => i.Cantidad * i.PrecioCompra);

                var numeroCompra = await _compraRepository.GenerarNumeroCompra();

                var compra = new Compra
                {
                    NumeroCompra = numeroCompra,
                    IdProveedor = request.IdProveedor,
                    IdUsuario = idUsuario,
                    FechaCompra = DateTime.Now,
                    Total = total,
                    Observaciones = request.Observaciones
                };

                var detalles = request.Items.Select(i => new DetalleCompra
                {
                    IdProducto = i.IdProducto,
                    Cantidad = i.Cantidad,
                    PrecioCompra = i.PrecioCompra,
                    Subtotal = i.Cantidad * i.PrecioCompra
                }).ToList();

                var compraCreada = await _compraRepository.Create(compra, detalles);

                return new ApiResponse<CompraDto>
                {
                    Success = true,
                    Message = $"Compra {numeroCompra} registrada exitosamente",
                    Data = MapToDto(compraCreada)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<CompraDto>
                {
                    Success = false,
                    Message = $"Error al crear compra: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<bool>> CancelarCompra(int id)
        {
            try
            {
                var compra = await _compraRepository.GetById(id);

                if (compra == null)
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Compra no encontrada"
                    };
                }

                var diasMaximos = 30;
                if ((DateTime.Now - compra.FechaCompra).TotalDays > diasMaximos)
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = $"No se pueden cancelar compras con más de {diasMaximos} días de antigüedad"
                    };
                }

                var cancelada = await _compraRepository.CancelarCompra(id);

                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = $"Compra {compra.NumeroCompra} cancelada exitosamente. Stock ajustado.",
                    Data = cancelada
                };
            }
            catch (InvalidOperationException ex)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Error al cancelar compra: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<Dictionary<string, object>>> GetEstadisticasHoy()
        {
            try
            {
                var fecha = DateTime.Now.Date;
                var totalCompras = await _compraRepository.GetTotalComprasPorFecha(fecha);
                var cantidadCompras = await _compraRepository.GetCantidadComprasPorFecha(fecha);

                var estadisticas = new Dictionary<string, object>
                {
                    { "fecha", fecha.ToString("dd/MM/yyyy") },
                    { "totalCompras", totalCompras },
                    { "cantidadCompras", cantidadCompras },
                    { "promedioCompra", cantidadCompras > 0 ? totalCompras / cantidadCompras : 0 }
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

        
        private CompraDto MapToDto(Compra compra)
        {
            return new CompraDto
            {
                IdCompra = compra.IdCompra,
                NumeroCompra = compra.NumeroCompra,
                IdProveedor = compra.IdProveedor,
                NombreProveedor = compra.Proveedor?.Nombre ?? "",
                IdUsuario = compra.IdUsuario,
                NombreUsuario = compra.Usuario?.NombreUsuario ?? "",
                FechaCompra = compra.FechaCompra,
                Total = compra.Total,
                Observaciones = compra.Observaciones,
                CantidadProductos = compra.DetallesCompra?.Sum(d => d.Cantidad) ?? 0,
                Detalles = compra.DetallesCompra?.Select(d => new DetalleCompraDto
                {
                    IdDetalleCompra = d.IdDetalleCompra,
                    IdProducto = d.IdProducto,
                    CodigoProducto = d.Producto?.Codigo ?? "",
                    NombreProducto = d.Producto?.Nombre ?? "",
                    Cantidad = d.Cantidad,
                    PrecioCompra = d.PrecioCompra,
                    Subtotal = d.Subtotal
                }).ToList() ?? new List<DetalleCompraDto>()
            };
        }

        private CompraResumenDto MapToResumenDto(Compra compra)
        {
            return new CompraResumenDto
            {
                IdCompra = compra.IdCompra,
                NumeroCompra = compra.NumeroCompra,
                NombreProveedor = compra.Proveedor?.Nombre ?? "",
                NombreUsuario = compra.Usuario?.NombreUsuario ?? "",
                FechaCompra = compra.FechaCompra,
                Total = compra.Total,
                CantidadProductos = compra.DetallesCompra?.Sum(d => d.Cantidad) ?? 0
            };
        }
    }
}