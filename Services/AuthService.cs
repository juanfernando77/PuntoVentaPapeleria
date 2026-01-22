using Microsoft.EntityFrameworkCore;
using PapeleriaAPI.Data;
using PapeleriaAPI.DTOs;
using PapeleriaAPI.Helpers;
using PapeleriaAPI.Models;

namespace PapeleriaAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtHelper _jwtHelper;

        public AuthService(ApplicationDbContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        public async Task<AuthResponse> Login(LoginRequest request)
        {
            try
            {
                // Buscar usuario por email
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == request.Email);

                if (usuario == null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Usuario incorrecta"
                    };
                }

                // Verificar contraseña
                if (!BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash))
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Contraseña incorrecta"
                    };
                }

                // Verificar si está activo
                if (!usuario.Activo)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Usuario inactivo. Contacte al administrador."
                    };
                }

                // Generar token
                var token = _jwtHelper.GenerarToken(usuario);

                return new AuthResponse
                {
                    Success = true,
                    Message = "Login exitoso",
                    Token = token,
                    Usuario = new UsuarioDto
                    {
                        IdUsuario = usuario.IdUsuario,
                        NombreUsuario = usuario.NombreUsuario,
                        Email = usuario.Email,
                        Rol = usuario.Rol,
                        Activo = usuario.Activo
                    }
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = $"Error en el login: {ex.Message}"
                };
            }
        }

        public async Task<AuthResponse> Register(RegisterRequest request)
        {
            try
            {
                // Verificar si el email ya existe
                var existeEmail = await _context.Usuarios
                    .AnyAsync(u => u.Email == request.Email);

                if (existeEmail)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "El email ya está registrado"
                    };
                }

                // Validar rol
                var rolesValidos = new[] { "Admin", "Cajero", "Almacenista" };
                if (!rolesValidos.Contains(request.Rol))
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Rol inválido. Roles válidos: Admin, Cajero, Almacenista"
                    };
                }

                // Crear nuevo usuario
                var nuevoUsuario = new Usuario
                {
                    NombreUsuario = request.NombreUsuario,
                    Email = request.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    Rol = request.Rol,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                };

                _context.Usuarios.Add(nuevoUsuario);
                await _context.SaveChangesAsync();

                // Generar token
                var token = _jwtHelper.GenerarToken(nuevoUsuario);

                return new AuthResponse
                {
                    Success = true,
                    Message = "Usuario registrado exitosamente",
                    Token = token,
                    Usuario = new UsuarioDto
                    {
                        IdUsuario = nuevoUsuario.IdUsuario,
                        NombreUsuario = nuevoUsuario.NombreUsuario,
                        Email = nuevoUsuario.Email,
                        Rol = nuevoUsuario.Rol,
                        Activo = nuevoUsuario.Activo
                    }
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = $"Error en el registro: {ex.Message}"
                };
            }
        }
    }
}