using Microsoft.AspNetCore.Mvc;
using ProfessionalAPI.DTOs.Auth;
using ProfessionalAPI.Services;

namespace ProfessionalAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest req)
    {
        var (ok, err) = await _auth.RegisterAsync(req);
        return ok ? Ok(new { message = "Usuario registrado correctamente." })
                  : BadRequest(new { error = err });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        var (ok, token, err) = await _auth.LoginAsync(req);
        return ok ? Ok(new { token })
                  : Unauthorized(new { error = err });
    }
}