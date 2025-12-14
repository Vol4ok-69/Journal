using JournalApi.DTOs.Auth;

namespace JournalApi.Interfaces;

public interface IAuthService
{
    Task<TokenResponseDTO?> AuthenticateAsync(string email, string password);
    bool RegisterUser(string email, string password, string fullName, out string message);

    Task<TokenResponseDTO?> RefreshTokenAsync(string refreshToken);
    Task<bool> RevokeRefreshTokenAsync(string refreshToken);
}
