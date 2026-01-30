using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PapeleriaAPI.DTOs;
using PapeleriaAPI.Services;

namespace PapeleriaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProveedorController : ControllerBase
    {
        private readonly IProveedorService _proveedorService;

        public ProveedorController(IProveedorService proveedorService)
        {
            _proveedorService = proveedorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _proveedorService.GetAll();

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
        [HttpGet("activos")]
        public async Task<IActionResult> GetAllActivos()
        {
            var response = await _proveedorService.GetAllActivos();

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _proveedorService.GetById(id);

            if (!response.Success)
                return NotFound(response);

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Almacenista")]
        public async Task<IActionResult> Create([FromBody] CreateProveedorRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _proveedorService.Create(request);

            if (!response.Success)
                return BadRequest(response);

            return CreatedAtAction(nameof(GetById), new { id = response.Data!.IdProveedor }, response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Almacenista")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProveedorRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _proveedorService.Update(id, request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _proveedorService.Delete(id);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
    }
}