using Agile_Aggregator.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IJwtTokenService _tokens;

    public AuthController(IJwtTokenService tokens)
    {
        _tokens = tokens;
    }

    /// <summary>
    /// POST /api/auth/login
    /// Body: { "userName": "admin", "password": "Password" }
    /// Returns: { "token": "{JWT…}" }
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public IActionResult Login([FromBody] LoginRequest req)
    {
        // TODO: replace with your real user‐store check
        if (req.UserName != "admin" || req.Password != "password")
            return Unauthorized();

        var jwt = _tokens.CreateToken(req.UserName, new[] { "Admin" });
        return Ok(new { token = jwt });
    }
}

public class LoginRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}