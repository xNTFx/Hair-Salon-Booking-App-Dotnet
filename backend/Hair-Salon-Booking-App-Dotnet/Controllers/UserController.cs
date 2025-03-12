using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly JwtUtil _jwtUtil;

    public UserController(IUserRepository userRepository, JwtUtil jwtUtil)
    {
        _userRepository = userRepository;
        _jwtUtil = jwtUtil;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        // Pobranie nagłówka Authorization
        if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            return Unauthorized(new { message = "Authorization header missing or invalid" });
        }

        var token = authHeader.ToString();
        if (!token.StartsWith("Bearer "))
        {
            return Unauthorized(new { message = "Authorization header missing or invalid" });
        }

        token = token.Substring("Bearer ".Length).Trim();

        ClaimsPrincipal claims;
        try
        {
            claims = _jwtUtil.ValidateAccessToken(token);
        }
        catch
        {
            return StatusCode(403, new { message = "Invalid or expired token" });
        }

        var username = claims.FindFirst("username")?.Value;
        if (string.IsNullOrEmpty(username))
        {
            return StatusCode(403, new { message = "Invalid or expired token" });
        }

        var user = await _userRepository.FindByUsernameAsync(username);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        return Ok(new
        {
            id = user.Id,
            username = user.Username,
            role = user.Role
        });
    }
}
