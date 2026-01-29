using PapeleriaAPI.DTOs;
using PapeleriaAPI.Models;
using PapeleriaAPI.Repositories;

namespace PapeleriaAPI.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepository;
        private readonly ICategoriaRepository _categoriaRepository;

        public ProductoService(
            IProductoRepository productoRepository,
            ICategoriaRepository categoriaRepository)
        {
            _productoRepository = productoRepository;
            _categoriaRepository = categoriaRepository;
        }

        public async Task<ApiResponse<IEnumerable<ProductoDto>>> GetAll()
        {
            try
            {
                var productos = await _productoRepository.GetAll();
                var productosDto = productos.Select(p => MapToDto(p));

                return new ApiResponse<IEnumerable<ProductoDto>>
                {
                    Success = true,
                    Message = "Productos obtenidos exitosamente",
                    Data = productosDto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<ProductoDto>>
                {
                    Success = false,
                    Message = $"Error al obtener productos: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<IEnumerable<ProductoDto>>> GetAllActivos()
        {
            try
            {
                var productos = await _productoRepository.GetAllActivos();
                var productosDto = productos.Select(p => MapToDto(p));

                return new ApiResponse<IEnumerable<ProductoDto>>
                {
                    Success = true,
                    Message = "Productos activos obtenidos exitosamente",
                    Data = productosDto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<ProductoDto>>
                {
                    Success = false,
                    Message = $"Error al obtener productos activos: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<IEnumerable<ProductoDto>>> GetByCategoria(int idCategoria)
        {
            try
            {
                // Verificar que la categoría existe
                if (!await _categoriaRepository.Exists(idCategoria))
                {
                    return new ApiResponse<IEnumerable<ProductoDto>>
                    {
                        Success = false,
                        Message = "Categoría no encontrada"
                    };
                }

                var productos = await _productoRepository.GetByCategoria(idCategoria);
                var productosDto = productos.Select(p => MapToDto(p));

                return new ApiResponse<IEnumerable<ProductoDto>>
                {
                    Success = true,
                    Message = "Productos obtenidos exitosamente",
                    Data = productosDto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<ProductoDto>>
                {
                    Success = false,
                    Message = $"Error al obtener productos por categoría: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<IEnumerable<ProductoDto>>> GetProductosBajoStock()
        {
            try
            {
                var productos = await _productoRepository.GetProductosBajoStock();
                var productosDto = productos.Select(p => MapToDto(p));

                return new ApiResponse<IEnumerable<ProductoDto>>
                {
                    Success = true,
                    Message = "Productos con stock bajo obtenidos exitosamente",
                    Data = productosDto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<ProductoDto>>
                {
                    Success = false,
                    Message = $"Error al obtener productos con stock bajo: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<ProductoDto>> GetById(int id)
        {
            try
            {
                var producto = await _productoRepository.GetById(id);

                if (producto == null)
                {
                    return new ApiResponse<ProductoDto>
                    {
                        Success = false,
                        Message = "Producto no encontrado"
                    };
                }

                return new ApiResponse<ProductoDto>
                {
                    Success = true,
                    Message = "Producto obtenido exitosamente",
                    Data = MapToDto(producto)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ProductoDto>
                {
                    Success = false,
                    Message = $"Error al obtener producto: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<ProductoDto>> GetByCodigo(string codigo)
        {
            try
            {
                var producto = await _productoRepository.GetByCodigo(codigo);

                if (producto == null)
                {
                    return new ApiResponse<ProductoDto>
                    {
                        Success = false,
                        Message = "Producto no encontrado"
                    };
                }

                return new ApiResponse<ProductoDto>
                {
                    Success = true,
                    Message = "Producto obtenido exitosamente",
                    Data = MapToDto(producto)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ProductoDto>
                {
                    Success = false,
                    Message = $"Error al obtener producto: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<ProductoDto>> Create(CreateProductoRequest request)
        {
            try
            {
                // Validar que la categoría existe
                if (!await _categoriaRepository.Exists(request.IdCategoria))
                {
                    return new ApiResponse<ProductoDto>
                    {
                        Success = false,
                        Message = "La categoría seleccionada no existe"
                    };
                }

                // Validar que el código no existe
                if (await _productoRepository.ExistsByCodigo(request.Codigo))
                {
                    return new ApiResponse<ProductoDto>
                    {
                        Success = false,
                        Message = "Ya existe un producto con ese código"
                    };
                }

                // Validar que el precio de venta sea mayor al de compra
                if (request.PrecioVenta <= request.PrecioCompra)
                {
                    return new ApiResponse<ProductoDto>
                    {
                        Success = false,
                        Message = "El precio de venta debe ser mayor al precio de compra"
                    };
                }

                var producto = new Producto
                {
                    Codigo = request.Codigo,
                    Nombre = request.Nombre,
                    Descripcion = request.Descripcion,
                    IdCategoria = request.IdCategoria,
                    PrecioCompra = request.PrecioCompra,
                    PrecioVenta = request.PrecioVenta,
                    Stock = request.Stock,
                    StockMinimo = request.StockMinimo,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                };

                var productoCreado = await _productoRepository.Create(producto);

                return new ApiResponse<ProductoDto>
                {
                    Success = true,
                    Message = "Producto creado exitosamente",
                    Data = MapToDto(productoCreado)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ProductoDto>
                {
                    Success = false,
                    Message = $"Error al crear producto: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<ProductoDto>> Update(int id, UpdateProductoRequest request)
        {
            try
            {
                var producto = await _productoRepository.GetById(id);

                if (producto == null)
                {
                    return new ApiResponse<ProductoDto>
                    {
                        Success = false,
                        Message = "Producto no encontrado"
                    };
                }

                // Validar que la categoría existe
                if (!await _categoriaRepository.Exists(request.IdCategoria))
                {
                    return new ApiResponse<ProductoDto>
                    {
                        Success = false,
                        Message = "La categoría seleccionada no existe"
                    };
                }

                // Validar que el código no existe en otro producto
                if (await _productoRepository.ExistsByCodigo(request.Codigo, id))
                {
                    return new ApiResponse<ProductoDto>
                    {
                        Success = false,
                        Message = "Ya existe otro producto con ese código"
                    };
                }

                // Validar que el precio de venta sea mayor al de compra
                if (request.PrecioVenta <= request.PrecioCompra)
                {
                    return new ApiResponse<ProductoDto>
                    {
                        Success = false,
                        Message = "El precio de venta debe ser mayor al precio de compra"
                    };
                }

                producto.Codigo = request.Codigo;
                producto.Nombre = request.Nombre;
                producto.Descripcion = request.Descripcion;
                producto.IdCategoria = request.IdCategoria;
                producto.PrecioCompra = request.PrecioCompra;
                producto.PrecioVenta = request.PrecioVenta;
                producto.Stock = request.Stock;
                producto.StockMinimo = request.StockMinimo;
                producto.Activo = request.Activo;

                var productoActualizado = await _productoRepository.Update(producto);

                return new ApiResponse<ProductoDto>
                {
                    Success = true,
                    Message = "Producto actualizado exitosamente",
                    Data = MapToDto(productoActualizado)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ProductoDto>
                {
                    Success = false,
                    Message = $"Error al actualizar producto: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<bool>> Delete(int id)
        {
            try
            {
                if (!await _productoRepository.Exists(id))
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Producto no encontrado"
                    };
                }

                var eliminado = await _productoRepository.Delete(id);

                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Producto eliminado exitosamente",
                    Data = eliminado
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Error al eliminar producto: {ex.Message}"
                };
            }
        }

        // Método auxiliar para mapear Producto a ProductoDto
        private ProductoDto MapToDto(Producto producto)
        {
            return new ProductoDto
            {
                IdProducto = producto.IdProducto,
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                IdCategoria = producto.IdCategoria,
                NombreCategoria = producto.Categoria?.Nombre ?? "",
                PrecioCompra = producto.PrecioCompra,
                PrecioVenta = producto.PrecioVenta,
                Stock = producto.Stock,
                StockMinimo = producto.StockMinimo,
                Activo = producto.Activo,
                FechaCreacion = producto.FechaCreacion,
                StockBajo = producto.Stock <= producto.StockMinimo
            };
        }
    }
}