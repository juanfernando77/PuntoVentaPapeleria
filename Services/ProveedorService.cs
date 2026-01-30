using PapeleriaAPI.DTOs;
using PapeleriaAPI.Models;
using PapeleriaAPI.Repositories;

namespace PapeleriaAPI.Services
{
    public class ProveedorService : IProveedorService
    {
        private readonly IProveedorRepository _proveedorRepository;

        public ProveedorService(IProveedorRepository proveedorRepository)
        {
            _proveedorRepository = proveedorRepository;
        }

        public async Task<ApiResponse<IEnumerable<ProveedorDto>>> GetAll()
        {
            try
            {
                var proveedores = await _proveedorRepository.GetAll();
                var proveedoresDto = proveedores.Select(p => MapToDto(p));

                return new ApiResponse<IEnumerable<ProveedorDto>>
                {
                    Success = true,
                    Message = "Proveedores obtenidos exitosamente",
                    Data = proveedoresDto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<ProveedorDto>>
                {
                    Success = false,
                    Message = $"Error al obtener proveedores: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<IEnumerable<ProveedorDto>>> GetAllActivos()
        {
            try
            {
                var proveedores = await _proveedorRepository.GetAllActivos();
                var proveedoresDto = proveedores.Select(p => MapToDto(p));

                return new ApiResponse<IEnumerable<ProveedorDto>>
                {
                    Success = true,
                    Message = "Proveedores activos obtenidos exitosamente",
                    Data = proveedoresDto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<ProveedorDto>>
                {
                    Success = false,
                    Message = $"Error al obtener proveedores activos: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<ProveedorDto>> GetById(int id)
        {
            try
            {
                var proveedor = await _proveedorRepository.GetById(id);

                if (proveedor == null)
                {
                    return new ApiResponse<ProveedorDto>
                    {
                        Success = false,
                        Message = "Proveedor no encontrado"
                    };
                }

                return new ApiResponse<ProveedorDto>
                {
                    Success = true,
                    Message = "Proveedor obtenido exitosamente",
                    Data = MapToDto(proveedor)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ProveedorDto>
                {
                    Success = false,
                    Message = $"Error al obtener proveedor: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<ProveedorDto>> Create(CreateProveedorRequest request)
        {
            try
            {
                
                if (!string.IsNullOrWhiteSpace(request.RFC))
                {
                    if (await _proveedorRepository.ExistsByRFC(request.RFC))
                    {
                        return new ApiResponse<ProveedorDto>
                        {
                            Success = false,
                            Message = "Ya existe un proveedor con ese RFC"
                        };
                    }
                }

                var proveedor = new Proveedor
                {
                    Nombre = request.Nombre,
                    RFC = request.RFC?.ToUpper(),
                    Telefono = request.Telefono,
                    Email = request.Email,
                    Direccion = request.Direccion,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                };

                var proveedorCreado = await _proveedorRepository.Create(proveedor);

                return new ApiResponse<ProveedorDto>
                {
                    Success = true,
                    Message = "Proveedor creado exitosamente",
                    Data = MapToDto(proveedorCreado)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ProveedorDto>
                {
                    Success = false,
                    Message = $"Error al crear proveedor: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<ProveedorDto>> Update(int id, UpdateProveedorRequest request)
        {
            try
            {
                var proveedor = await _proveedorRepository.GetById(id);

                if (proveedor == null)
                {
                    return new ApiResponse<ProveedorDto>
                    {
                        Success = false,
                        Message = "Proveedor no encontrado"
                    };
                }

                if (!string.IsNullOrWhiteSpace(request.RFC))
                {
                    if (await _proveedorRepository.ExistsByRFC(request.RFC, id))
                    {
                        return new ApiResponse<ProveedorDto>
                        {
                            Success = false,
                            Message = "Ya existe otro proveedor con ese RFC"
                        };
                    }
                }

                proveedor.Nombre = request.Nombre;
                proveedor.RFC = request.RFC?.ToUpper();
                proveedor.Telefono = request.Telefono;
                proveedor.Email = request.Email;
                proveedor.Direccion = request.Direccion;
                proveedor.Activo = request.Activo;

                var proveedorActualizado = await _proveedorRepository.Update(proveedor);

                return new ApiResponse<ProveedorDto>
                {
                    Success = true,
                    Message = "Proveedor actualizado exitosamente",
                    Data = MapToDto(proveedorActualizado)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ProveedorDto>
                {
                    Success = false,
                    Message = $"Error al actualizar proveedor: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<bool>> Delete(int id)
        {
            try
            {
                if (!await _proveedorRepository.Exists(id))
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Proveedor no encontrado"
                    };
                }

                var eliminado = await _proveedorRepository.Delete(id);

                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Proveedor eliminado exitosamente",
                    Data = eliminado
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Error al eliminar proveedor: {ex.Message}"
                };
            }
        }
        private ProveedorDto MapToDto(Proveedor proveedor)
        {
            return new ProveedorDto
            {
                IdProveedor = proveedor.IdProveedor,
                Nombre = proveedor.Nombre,
                RFC = proveedor.RFC,
                Telefono = proveedor.Telefono,
                Email = proveedor.Email,
                Direccion = proveedor.Direccion,
                Activo = proveedor.Activo,
                FechaCreacion = proveedor.FechaCreacion
            };
        }
    }
}