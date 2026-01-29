using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PapeleriaAPI.DTOs;
using PapeleriaAPI.Services;

namespace PapeleriaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Requiere autenticación para todos los endpoints
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        /// <summary>
        /// Obtener todas las categorías
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _categoriaService.GetAll();

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Obtener solo las categorías activas
        /// </summary>
        [HttpGet("activas")]
        public async Task<IActionResult> GetAllActivas()
        {
            var response = await _categoriaService.GetAllActivas();

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Obtener una categoría por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _categoriaService.GetById(id);

            if (!response.Success)
                return NotFound(response);

            return Ok(response);
        }

        /// <summary>
        /// Crear una nueva categoría
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")] // Solo admin puede crear
        public async Task<IActionResult> Create([FromBody] CreateCategoriaRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _categoriaService.Create(request);

            if (!response.Success)
                return BadRequest(response);

            return CreatedAtAction(nameof(GetById), new { id = response.Data!.IdCategoria }, response);
        }

        /// <summary>
        /// Actualizar una categoría existente
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Solo admin puede actualizar
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoriaRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _categoriaService.Update(id, request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Eliminar una categoría (eliminación lógica)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Solo admin puede eliminar
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _categoriaService.Delete(id);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
    }
}