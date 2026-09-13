using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LicenciaBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config)
    {
        _config = config;
    }

    public record LoginRequest(string Username, string Password);
    public record LoginResponse(string Token, DateTime ExpiresAt);

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // ⚠️ Acá podés luego consultar DB/Identity.
        // Por ahora, simple:
        var adminUser = _config["LicenciaTool:AdminUser"] ?? "admin";
        var adminPass = _config["LicenciaTool:AdminPassword"] ?? "supersecreto";

        if (!string.Equals(request.Username, adminUser, StringComparison.OrdinalIgnoreCase) ||
            request.Password != adminPass)
        {
            return Unauthorized("Usuario o contraseña inválidos.");
        }

        var jwtKey = _config["Jwt:Key"]!;
        var jwtIssuer = _config["Jwt:Issuer"]!;
        var jwtAudience = _config["Jwt:Audience"]!;
        var minutes = int.TryParse(_config["Jwt:TokenLifetimeMinutes"], out var m) ? m : 60;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, request.Username),
            new Claim(ClaimTypes.Role, "LicenciasAdmin")
        };

        var expires = DateTime.UtcNow.AddMinutes(minutes);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new LoginResponse(tokenString, expires));
    }
}
