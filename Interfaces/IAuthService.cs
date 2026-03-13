using JournalApi.DTOs.Auth;

namespace JournalApi.Interfaces;

public interface IAuthService
{
    Task<TokenResponseDTO> LoginAsync(LoginRequestDTO request);
    Task<TokenResponseDTO?> RefreshTokenAsync(RefreshTokenRequestDTO request);
    Task<RegisterResponseDTO> RegisterAsync(RegisterRequestDTO request);
    Task<bool> LogoutAsync(string refreshToken);
    Task LogEventAsync(string message, string type = "Info");
}