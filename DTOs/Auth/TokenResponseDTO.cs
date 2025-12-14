namespace JournalApi.DTOs.Auth
{
    public class TokenResponseDTO
    {
        public string Login { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Patronymic { get; set; }
        public string Role { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
