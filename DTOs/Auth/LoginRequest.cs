using System.ComponentModel.DataAnnotations;

namespace ProfessionalAPI.DTOs.Auth;

public record LoginRequest(
    [Required][EmailAddress] string Email,
    [Required][MinLength(6)] string Password
);

public record RegisterRequest(
    [Required] string Name,
    [Required][EmailAddress] string Email,
    [Required][MinLength(6)] string Password
);