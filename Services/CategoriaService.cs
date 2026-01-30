using PapeleriaAPI.DTOs;
using PapeleriaAPI.Models;
using PapeleriaAPI.Repositories;

namespace PapeleriaAPI.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepository;

        public CategoriaService(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        public async Task<ApiResponse<IEnumerable<CategoriaDto>>> GetAll()
        {
            try
            {
                var categorias = await _categoriaRepository.GetAll();
                var categoriasDto = categorias.Select(c => MapToDto(c));

                return new ApiResponse<IEnumerable<CategoriaDto>>
                {
                    Success = true,
                    Message = "Categorías obtenidas exitosamente",
                    Data = categoriasDto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<CategoriaDto>>
                {
                    Success = false,
                    Message = $"Error al obtener categorías: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<IEnumerable<CategoriaDto>>> GetAllActivas()
        {
            try
            {
                var categorias = await _categoriaRepository.GetAllActivas();
                var categoriasDto = categorias.Select(c => MapToDto(c));

                return new ApiResponse<IEnumerable<CategoriaDto>>
                {
                    Success = true,
                    Message = "Categorías activas obtenidas exitosamente",
                    Data = categoriasDto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<CategoriaDto>>
                {
                    Success = false,
                    Message = $"Error al obtener categorías activas: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<CategoriaDto>> GetById(int id)
        {
            try
            {
                var categoria = await _categoriaRepository.GetById(id);

                if (categoria == null)
                {
                    return new ApiResponse<CategoriaDto>
                    {
                        Success = false,
                        Message = "Categoría no encontrada"
                    };
                }

                return new ApiResponse<CategoriaDto>
                {
                    Success = true,
                    Message = "Categoría obtenida exitosamente",
                    Data = MapToDto(categoria)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<CategoriaDto>
                {
                    Success = false,
                    Message = $"Error al obtener categoría: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<CategoriaDto>> Create(CreateCategoriaRequest request)
        {
            try
            {
                if (await _categoriaRepository.ExistsByNombre(request.Nombre))
                {
                    return new ApiResponse<CategoriaDto>
                    {
                        Success = false,
                        Message = "Ya existe una categoría con ese nombre"
                    };
                }

                var categoria = new Categoria
                {
                    Nombre = request.Nombre,
                    Descripcion = request.Descripcion,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                };

                var categoriaCreada = await _categoriaRepository.Create(categoria);

                return new ApiResponse<CategoriaDto>
                {
                    Success = true,
                    Message = "Categoría creada exitosamente",
                    Data = MapToDto(categoriaCreada)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<CategoriaDto>
                {
                    Success = false,
                    Message = $"Error al crear categoría: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<CategoriaDto>> Update(int id, UpdateCategoriaRequest request)
        {
            try
            {
                var categoria = await _categoriaRepository.GetById(id);

                if (categoria == null)
                {
                    return new ApiResponse<CategoriaDto>
                    {
                        Success = false,
                        Message = "Categoría no encontrada"
                    };
                }
                if (await _categoriaRepository.ExistsByNombre(request.Nombre, id))
                {
                    return new ApiResponse<CategoriaDto>
                    {
                        Success = false,
                        Message = "Ya existe otra categoría con ese nombre"
                    };
                }

                categoria.Nombre = request.Nombre;
                categoria.Descripcion = request.Descripcion;
                categoria.Activo = request.Activo;

                var categoriaActualizada = await _categoriaRepository.Update(categoria);

                return new ApiResponse<CategoriaDto>
                {
                    Success = true,
                    Message = "Categoría actualizada exitosamente",
                    Data = MapToDto(categoriaActualizada)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<CategoriaDto>
                {
                    Success = false,
                    Message = $"Error al actualizar categoría: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<bool>> Delete(int id)
        {
            try
            {
                if (!await _categoriaRepository.Exists(id))
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Categoría no encontrada"
                    };
                }

                var eliminada = await _categoriaRepository.Delete(id);

                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Categoría eliminada exitosamente",
                    Data = eliminada
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Error al eliminar categoría: {ex.Message}"
                };
            }
        }

        private CategoriaDto MapToDto(Categoria categoria)
        {
            return new CategoriaDto
            {
                IdCategoria = categoria.IdCategoria,
                Nombre = categoria.Nombre,
                Descripcion = categoria.Descripcion,
                Activo = categoria.Activo,
                FechaCreacion = categoria.FechaCreacion
            };
        }
    }
}