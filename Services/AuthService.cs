using JournalApi.DTOs.Auth;
using JournalApi.Helpers;
using JournalApi.Interfaces;
using System.Security.Claims;
using BCryptNet = BCrypt.Net.BCrypt;

namespace JournalApi.Services;

public class AuthService : IAuthService
{
    private readonly DataBaseContext _context;
    private readonly IConfiguration _configuration;
    private readonly string _secretKey;
    private readonly int _tokenExpirationMinutes;
    private readonly int _refreshTokenDays;
    private readonly string _issuer;
    private readonly string _audience;

    public AuthService(DataBaseContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
        _secretKey = _configuration["Jwt:SecretKey"] ?? throw new Exception("JWT SecretKey is not configured");
        _tokenExpirationMinutes = int.Parse(_configuration["Jwt:TokenExpirationMinutes"] ?? "15");
        _refreshTokenDays = int.Parse(_configuration["Jwt:RefreshTokenDays"] ?? "14");
        _issuer = _configuration["Jwt:Issuer"] ?? "JournalApi";
        _audience = _configuration["Jwt:Audience"] ?? "JournalClient";
    }

    public async Task<TokenResponseDTO> LoginAsync(LoginRequestDTO request)
    {
        try
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Login == request.Login);
            Employee? employee = null;
            if (student == null)
            {
                employee = await _context.Employees.FirstOrDefaultAsync(e => e.Login == request.Login);
            }

            if (student != null)
            {
                if (!BCryptNet.Verify(request.Password, student.Password))
                {
                    await LogEventAsync($"Failed login attempt for student: {request.Login}. Password is incorrect.", "Error");
                    return new TokenResponseDTO { Message = $"Password for student: {request.Login} is incorrect." };
                }

                List<Claim> claims =
                [
                    new("Id", student.Id.ToString()),
                    new("Login", student.Login ?? ""),
                    new("Surname", student.Surname),
                    new("Name", student.Name),
                    new("Role", "Student")
                ];

                var accessToken = JwtHelper.GenerateAccessToken(claims, _secretKey, _issuer, _audience, _tokenExpirationMinutes);
                var refreshToken = JwtHelper.GenerateRefreshToken();

                var refreshTokenEntity = new RefreshToken
                {
                    Token = refreshToken,
                    UserId = student.Id,
                    UserLogin = student.Login ?? "",
                    UserRole = "Student",
                    ExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenDays)
                };

                _context.RefreshTokens.Add(refreshTokenEntity);
                await _context.SaveChangesAsync();

                await LogEventAsync($"Successful login for student: {request.Login}", "Info");

                return new TokenResponseDTO { AccessToken = accessToken, RefreshToken = refreshToken };
            }
            else if (employee != null)
            {
                if (!BCryptNet.Verify(request.Password, employee.Password))
                {
                    await LogEventAsync($"Failed login attempt for employee: {request.Login}. Password is incorrect.", "Error");
                    return new TokenResponseDTO { Message = $"Password for employee: {request.Login} is incorrect." };
                }

                var post = await _context.EmployeePosts.FirstOrDefaultAsync(p => p.Id == employee.PostId);
                string role = "Employee";
                if (post?.Post.Contains("Администратор", StringComparison.OrdinalIgnoreCase) == true)
                {
                    role = "Admin";
                }
                else if (post?.Post.Contains("Куратор", StringComparison.OrdinalIgnoreCase) == true)
                {
                    role = "Curator";
                }
                else if (post?.Post.Contains("Преподаватель", StringComparison.OrdinalIgnoreCase) == true)
                {
                    role = "Teacher";
                }

                List<Claim> claims =
                [
                    new("Id", employee.Id.ToString()),
                    new("Login", employee.Login),
                    new("Surname", employee.Surname),
                    new("Name", employee.Name),
                    new("Role", role)
                ];

                var accessToken = JwtHelper.GenerateAccessToken(claims, _secretKey, _issuer, _audience, _tokenExpirationMinutes);
                var refreshToken = JwtHelper.GenerateRefreshToken();

                var refreshTokenEntity = new RefreshToken
                {
                    Token = refreshToken,
                    UserId = employee.Id,
                    UserLogin = employee.Login,
                    UserRole = role,
                    ExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenDays)
                };

                _context.RefreshTokens.Add(refreshTokenEntity);
                await _context.SaveChangesAsync();

                await LogEventAsync($"Successful login for employee: {request.Login} (Role: {role})", "Info");

                return new TokenResponseDTO { AccessToken = accessToken, RefreshToken = refreshToken, Message = $"Successful login for employee: {request.Login} (Role: {role})" };
            }

            await LogEventAsync($"Login attempt for non-existent user: {request.Login}", "Warning");
            return new TokenResponseDTO { Message = $"Login attempt for non-existent user: {request.Login}" };
        }
        catch (Exception ex)
        {
            await LogEventAsync($"Exception during login for {request.Login}: {ex.Message}", "Error");
            return new TokenResponseDTO { Message = $"Exception during login for {request.Login}: {ex.Message}" };
        }
    }

    public async Task<TokenResponseDTO?> RefreshTokenAsync(RefreshTokenRequestDTO request)
    {
        try
        {
            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

            if (storedToken == null)
            {
                await LogEventAsync($"Refresh token not found in DB: {request.RefreshToken}", "Error");
                return new TokenResponseDTO { Message = $"Refresh token not found in DB: {request.RefreshToken}" };
            }

            if (storedToken.ExpiresAt < DateTime.UtcNow)
            {
                _context.RefreshTokens.Remove(storedToken);
                await _context.SaveChangesAsync();
                await LogEventAsync($"Attempt to use expired refresh token for user: {storedToken.UserLogin}", "Error");
                return new TokenResponseDTO { Message = $"Attempt to use expired refresh token for user: {storedToken.UserLogin}" };
            }

            if (storedToken.RevokedAt != null)
            {
                await LogEventAsync($"Attempt to use revoked refresh token for user: {storedToken.UserLogin}", "Error");
                return new TokenResponseDTO { Message = $"Attempt to use revoked refresh token for user: {storedToken.UserLogin}" };
            }

            var newAccessToken = JwtHelper.GenerateAccessToken(
                [
                    new("Id", storedToken.UserId.ToString()),
                    new("Login", storedToken.UserLogin),
                    new("Role", storedToken.UserRole)
                ],
                _secretKey, _issuer, _audience, _tokenExpirationMinutes
            );

            var newRefreshToken = JwtHelper.GenerateRefreshToken();

            _context.RefreshTokens.Remove(storedToken);

            var newTokenEntity = new RefreshToken
            {
                Token = newRefreshToken,
                UserId = storedToken.UserId,
                UserLogin = storedToken.UserLogin,
                UserRole = storedToken.UserRole,
                ExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenDays)
            };

            _context.RefreshTokens.Add(newTokenEntity);
            await _context.SaveChangesAsync();

            await LogEventAsync($"Refreshed token for user: {storedToken.UserLogin} (ID: {storedToken.UserId}, Role: {storedToken.UserRole})", "Info");

            return new TokenResponseDTO { AccessToken = newAccessToken, RefreshToken = newRefreshToken };
        }
        catch (Exception ex)
        {
            await LogEventAsync($"Exception during token refresh: {ex.Message}", "Error");
            return new TokenResponseDTO { Message = $"Exception during token refresh: {ex.Message}" };
        }
    }

    public async Task<RegisterResponseDTO> RegisterAsync(RegisterRequestDTO request)
    {
        try
        {
            var existingStudent = await _context.Students.AnyAsync(s => s.Login == request.Login);
            var existingEmployee = await _context.Employees.AnyAsync(e => e.Login == request.Login);

            if (existingStudent || existingEmployee)
            {
                await LogEventAsync($"Registration failed: Login '{request.Login}' already exists.", "Warning");
                return new RegisterResponseDTO { Result = false, Message = $"Registration failed: Login '{request.Login}' already exists." };
            }

            if (request.Role == "Student")
            {
                if (!request.GroupId.HasValue)
                {
                    await LogEventAsync($"Registration failed for student: GroupId is required.", "Error");
                    return new RegisterResponseDTO { Result = false, Message = $"Registration failed for student: GroupId is required." };
                }

                var student = new Student
                {
                    Surname = request.Surname,
                    Name = request.Name,
                    Patronymic = request.Patronymic,
                    Birthday = request.Birthday,
                    Login = request.Login,
                    Password = BCryptNet.HashPassword(request.Password),
                    GroupId = request.GroupId.Value,
                    Phone = request.Phone
                };

                _context.Students.Add(student);
                await _context.SaveChangesAsync();
                await LogEventAsync($"New student registered: {request.Login}", "Info");
                return new RegisterResponseDTO { Result = true, Message = $"New student registered: {request.Login}" };
            }
            else if (request.Role == "Teacher" || request.Role == "Curator" || request.Role == "Admin")
            {
                if (!request.PostId.HasValue)
                {
                    await LogEventAsync($"Registration failed for employee: PostId is required.", "Error");
                    return new RegisterResponseDTO { Result = false, Message = $"Registration failed for employee: PostId is required." };
                }

                var employee = new Employee
                {
                    Surname = request.Surname,
                    Name = request.Name,
                    Patronymic = request.Patronymic,
                    Birthday = request.Birthday,
                    Login = request.Login,
                    Password = BCryptNet.HashPassword(request.Password),
                    Phone = request.Phone,
                    PostId = request.PostId.Value,
                    Salary = request.Salary ?? 0
                };

                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
                await LogEventAsync($"New employee registered: {request.Login} (Role: {request.Role})", "Info");
                return new RegisterResponseDTO { Result = true, Message = $"New employee registered: {request.Login} (Role: {request.Role})" };
            }

            await LogEventAsync($"Registration failed: Unknown role '{request.Role}' for user '{request.Login}'.", "Error");
            return new RegisterResponseDTO { Result = false, Message = $"Registration failed: Unknown role '{request.Role}' for user '{request.Login}'." };
        }
        catch (Exception ex)
        {
            await LogEventAsync($"Exception during registration for {request.Login}: {ex.Message}", "Error");
            return new RegisterResponseDTO { Result = false, Message = $"Exception during registration for {request.Login}: {ex.Message}" };
        }
    }

    public async Task<bool> LogoutAsync(string refreshToken)
    {
        try
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (token != null && token.RevokedAt == null)
            {
                token.RevokedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                await LogEventAsync($"User logged out. Refresh token revoked for user: {token.UserLogin}", "Info");
                return true;
            }

            await LogEventAsync($"Logout attempted with non-existent or already revoked token: {refreshToken}", "Warning");
            return false;
        }
        catch (Exception ex)
        {
            await LogEventAsync($"Exception during logout for token {refreshToken}: {ex.Message}", "Error");
            return false;
        }
    }

    public async Task LogEventAsync(string message, string type = "Info")
    {
        var log = new Log
        {
            Date = DateTime.UtcNow,
            Message = message,
            Type = type
        };

        _context.Logs.Add(log);
        await _context.SaveChangesAsync();
    }
}