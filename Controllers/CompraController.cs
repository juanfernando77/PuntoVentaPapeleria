using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PapeleriaAPI.DTOs;
using PapeleriaAPI.Services;
using System.Security.Claims;

namespace PapeleriaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompraController : ControllerBase
    {
        private readonly ICompraService _compraService;

        public CompraController(ICompraService compraService)
        {
            _compraService = compraService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Almacenista")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _compraService.GetAll();

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("fecha/{fecha}")]
        [Authorize(Roles = "Admin,Almacenista")]
        public async Task<IActionResult> GetByFecha(DateTime fecha)
        {
            var response = await _compraService.GetByFecha(fecha);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("proveedor/{idProveedor}")]
        [Authorize(Roles = "Admin,Almacenista")]
        public async Task<IActionResult> GetByProveedor(int idProveedor)
        {
            var response = await _compraService.GetByProveedor(idProveedor);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("periodo")]
        [Authorize(Roles = "Admin,Almacenista")]
        public async Task<IActionResult> GetByPeriodo([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            var response = await _compraService.GetByPeriodo(fechaInicio, fechaFin);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Almacenista")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _compraService.GetById(id);

            if (!response.Success)
                return NotFound(response);

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Almacenista")]
        public async Task<IActionResult> Create([FromBody] CreateCompraRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "No se pudo identificar al usuario" });
            }

            var idUsuario = int.Parse(userIdClaim.Value);

            var response = await _compraService.Create(request, idUsuario);

            if (!response.Success)
                return BadRequest(response);

            return CreatedAtAction(nameof(GetById), new { id = response.Data!.IdCompra }, response);
        }

        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CancelarCompra(int id)
        {
            var response = await _compraService.CancelarCompra(id);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        
        [HttpGet("estadisticas/hoy")]
        [Authorize(Roles = "Admin,Almacenista")]
        public async Task<IActionResult> GetEstadisticasHoy()
        {
            var response = await _compraService.GetEstadisticasHoy();

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
    }
}