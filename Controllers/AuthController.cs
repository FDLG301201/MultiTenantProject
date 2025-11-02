using BCrypt.Net;
using Infraestructura.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly TokenService _token;

    public AuthController(AppDbContext db, TokenService token)
    {
        _db = db; _token = token;
    }

    [HttpPost("Login")]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        var user = _db.Users.SingleOrDefault(u => u.Username == dto.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized();

        var token = _token.GenerateToken(user);
        return Ok(new { token });
    }

    [Authorize]
    [HttpPost("CambioDeClave")]
    public IActionResult ChangePassword(ChangePasswordDto dto)
    {
        var username = User.FindFirst("username")?.Value;
        var user = _db.Users.SingleOrDefault(u => u.Username == username);
        if (user == null) return Unauthorized();
        if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            return BadRequest("Contraseña incorrecta");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        _db.SaveChanges();
        return Ok("Contraseña actualizada");
    }

    [HttpPost("OlvideMiClave")]
    public IActionResult ForgotPassword(ForgotDto dto)
    {
        Log.Information("Solicitud de recuperación: {User}", dto.UserOrEmail);
        return Ok("Solicitud registrada (ver logs)");
    }
}

public record LoginDto(string Username, string Password);
public record ChangePasswordDto(string CurrentPassword, string NewPassword);
public record ForgotDto(string UserOrEmail);
