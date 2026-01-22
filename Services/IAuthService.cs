using PapeleriaAPI.DTOs;

namespace PapeleriaAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> Login(LoginRequest request);
        Task<AuthResponse> Register(RegisterRequest request);
    }
}