// Лучше переименуйте файл в RefreshTokenRequestDTO.cs
namespace JournalApi.DTOs.Auth;

public class RefreshTokenRequestDTO // Или RefreshTokenRequestDTO
{
    public string RefreshToken { get; set; } = null!;
}