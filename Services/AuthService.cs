using ProfessionalAPI.Data;
using ProfessionalAPI.DTOs.Auth;
using ProfessionalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ProfessionalAPI.Services;

public interface IAuthService
{
    Task<(bool Success, string Token, string Error)> LoginAsync(LoginRequest req);
    Task<(bool Success, string Error)> RegisterAsync(RegisterRequest req);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly ITokenService _tokenSvc;

    public AuthService(AppDbContext db, ITokenService tokenSvc)
    {
        _db = db;
        _tokenSvc = tokenSvc;
    }

    public async Task<(bool, string, string)> LoginAsync(LoginRequest req)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == req.Email && u.IsActive);

        if (user is null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            return (false, string.Empty, "Credenciales inválidas.");

        return (true, _tokenSvc.GenerateToken(user), string.Empty);
    }

    public async Task<(bool, string)> RegisterAsync(RegisterRequest req)
    {
        if (await _db.Users.AnyAsync(u => u.Email == req.Email))
            return (false, "El email ya está registrado.");

        var user = new User
        {
            Name = req.Name,
            Email = req.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return (true, string.Empty);
    }
}