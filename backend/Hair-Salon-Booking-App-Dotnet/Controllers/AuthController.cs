using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using HairSalonBookingApp.Models;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly JwtUtil _jwtUtil;

    public AuthController(AuthService authService, JwtUtil jwtUtil)
    {
        _authService = authService;
        _jwtUtil = jwtUtil;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthRequest request)
    {
        var user = await _authService.FindUserByUsernameAsync(request.Username);
        if (user == null)
            return Unauthorized(new { message = "invalid credentials" });

        var accessToken = _jwtUtil.GenerateAccessToken(user.Username, user.Id, user.Role);
        var refreshToken = _jwtUtil.GenerateRefreshToken(user.Username);
        await _authService.UpdateRefreshTokenAsync(user.Id, refreshToken);

        Response.Cookies.Append("jwt", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });

        return Ok(new { accessToken, user });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AuthRequest request)
    {
        if (await _authService.CheckDuplicateUsernameAsync(request.Username))
            return Conflict(new { message = "username already exists" });

        var user = await _authService.CreateUserAsync(request.Username, request.Password, "user");

        //  logowanie po rejestracji:
        var accessToken = _jwtUtil.GenerateAccessToken(user.Username, user.Id, user.Role);
        var refreshToken = _jwtUtil.GenerateRefreshToken(user.Username);
        await _authService.UpdateRefreshTokenAsync(user.Id, refreshToken);

        Response.Cookies.Append("jwt", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });

        return Created("", new { accessToken, user });
    }


    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");
        return NoContent();
    }

    [HttpGet("refresh")]
    public async Task<IActionResult> RefreshToken()
    {
        if (!Request.Cookies.TryGetValue("jwt", out var refreshToken))
        {
            return Unauthorized(new { message = "refresh token missing." });
        }

        var user = await _authService.FindUserByRefreshTokenAsync(refreshToken);
        if (user == null)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "user not found for refresh token." });
        }


        ClaimsPrincipal claims;
        try
        {
            claims = _jwtUtil.ValidateRefreshToken(refreshToken);
        }
        catch
        {
            return Forbid("invalid refresh token.");
        }

        var tokenUsername = claims.FindFirst("username")?.Value;
        if (tokenUsername != user.Username)
        {
            return Forbid("token data does not match user.");
        }

        var newAccessToken = _jwtUtil.GenerateAccessToken(user.Username, user.Id, user.Role);
        return Ok(new { accessToken = newAccessToken });
    }
}
