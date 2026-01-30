using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PapeleriaAPI.DTOs;
using PapeleriaAPI.Services;

namespace PapeleriaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService _productoService;

        public ProductoController(IProductoService productoService)
        {
            _productoService = productoService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _productoService.GetAll();

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
        [HttpGet("activos")]
        public async Task<IActionResult> GetAllActivos()
        {
            var response = await _productoService.GetAllActivos();

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
        [HttpGet("categoria/{idCategoria}")]
        public async Task<IActionResult> GetByCategoria(int idCategoria)
        {
            var response = await _productoService.GetByCategoria(idCategoria);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
        [HttpGet("bajo-stock")]
        public async Task<IActionResult> GetProductosBajoStock()
        {
            var response = await _productoService.GetProductosBajoStock();

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _productoService.GetById(id);

            if (!response.Success)
                return NotFound(response);

            return Ok(response);
        }
        [HttpGet("codigo/{codigo}")]
        public async Task<IActionResult> GetByCodigo(string codigo)
        {
            var response = await _productoService.GetByCodigo(codigo);

            if (!response.Success)
                return NotFound(response);

            return Ok(response);
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Almacenista")]
        public async Task<IActionResult> Create([FromBody] CreateProductoRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _productoService.Create(request);

            if (!response.Success)
                return BadRequest(response);

            return CreatedAtAction(nameof(GetById), new { id = response.Data!.IdProducto }, response);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Almacenista")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductoRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _productoService.Update(id, request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _productoService.Delete(id);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
    }
}