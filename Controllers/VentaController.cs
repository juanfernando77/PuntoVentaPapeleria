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
    public class VentaController : ControllerBase
    {
        private readonly IVentaService _ventaService;

        public VentaController(IVentaService ventaService)
        {
            _ventaService = ventaService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Almacenista")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _ventaService.GetAll();

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("hoy")]
        public async Task<IActionResult> GetVentasHoy()
        {
            var response = await _ventaService.GetVentasHoy();

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("fecha/{fecha}")]
        [Authorize(Roles = "Admin,Almacenista")]
        public async Task<IActionResult> GetByFecha(DateTime fecha)
        {
            var response = await _ventaService.GetByFecha(fecha);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("periodo")]
        [Authorize(Roles = "Admin,Almacenista")]
        public async Task<IActionResult> GetByPeriodo([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            var response = await _ventaService.GetByPeriodo(fechaInicio, fechaFin);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

      
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _ventaService.GetById(id);

            if (!response.Success)
                return NotFound(response);

            return Ok(response);
        }

    
        [HttpPost]
        [Authorize(Roles = "Admin,Cajero")]
        public async Task<IActionResult> Create([FromBody] CreateVentaRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "No se pudo identificar al usuario" });
            }

            var idUsuario = int.Parse(userIdClaim.Value);

            var response = await _ventaService.Create(request, idUsuario);

            if (!response.Success)
                return BadRequest(response);

            return CreatedAtAction(nameof(GetById), new { id = response.Data!.IdVenta }, response);
        }

       
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CancelarVenta(int id)
        {
            var response = await _ventaService.CancelarVenta(id);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        
        [HttpGet("estadisticas/hoy")]
        public async Task<IActionResult> GetEstadisticasHoy()
        {
            var response = await _ventaService.GetEstadisticasHoy();

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
    }
}