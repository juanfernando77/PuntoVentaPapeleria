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
    public class CorteCajaController : ControllerBase
    {
        private readonly ICorteCajaService _corteCajaService;

        public CorteCajaController(ICorteCajaService corteCajaService)
        {
            _corteCajaService = corteCajaService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _corteCajaService.GetAll();

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Cajero")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _corteCajaService.GetById(id);

            if (!response.Success)
                return NotFound(response);

            return Ok(response);
        }

        [HttpGet("activo")]
        public async Task<IActionResult> GetCorteActivo()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "No se pudo identificar al usuario" });
            }

            var idUsuario = int.Parse(userIdClaim.Value);
            var response = await _corteCajaService.GetCorteActivo(idUsuario);

            if (!response.Success)
                return NotFound(response);

            return Ok(response);
        }

        [HttpGet("mis-cortes")]
        public async Task<IActionResult> GetMisCortes()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "No se pudo identificar al usuario" });
            }

            var idUsuario = int.Parse(userIdClaim.Value);
            var response = await _corteCajaService.GetByUsuario(idUsuario);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("fecha/{fecha}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByFecha(DateTime fecha)
        {
            var response = await _corteCajaService.GetByFecha(fecha);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("abrir")]
        [Authorize(Roles = "Admin,Cajero")]
        public async Task<IActionResult> AbrirCaja([FromBody] AbrirCajaRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "No se pudo identificar al usuario" });
            }

            var idUsuario = int.Parse(userIdClaim.Value);
            var response = await _corteCajaService.AbrirCaja(request, idUsuario);

            if (!response.Success)
                return BadRequest(response);

            return CreatedAtAction(nameof(GetById), new { id = response.Data!.IdCorte }, response);
        }

        [HttpPut("{id}/cerrar")]
        [Authorize(Roles = "Admin,Cajero")]
        public async Task<IActionResult> CerrarCaja(int id, [FromBody] CerrarCajaRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _corteCajaService.CerrarCaja(id, request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
    }
}