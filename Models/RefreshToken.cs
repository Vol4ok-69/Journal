using System;
using System.Collections.Generic;

namespace JournalApi.Models;

public partial class RefreshToken
{
    public int Id { get; set; }

    public string Token { get; set; } = null!;

    public int UserId { get; set; }

    public string UserLogin { get; set; } = null!;

    public string UserRole { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? RevokedAt { get; set; }
}
