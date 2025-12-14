// Interfaces/IAuthService.cs
using JournalApi.DTOs.Auth;

namespace JournalApi.Interfaces;

public interface IAuthService
{
    Task<TokenResponseDTO?> LoginAsync(LoginRequestDTO request);
    Task<TokenResponseDTO?> RefreshTokenAsync(RefreshTokenRequestDTO request);
    Task<bool> RegisterAsync(RegisterRequestDTO request);
    Task LogoutAsync();
}