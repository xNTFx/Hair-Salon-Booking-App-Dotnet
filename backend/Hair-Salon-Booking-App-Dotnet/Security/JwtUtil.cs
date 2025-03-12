using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class JwtUtil
{
    private readonly string _accessTokenSecret;
    private readonly string _refreshTokenSecret;
    private readonly int _accessTokenExpiration;
    private readonly int _refreshTokenExpiration;
    private readonly ILogger<JwtUtil> _logger;

    public JwtUtil(IConfiguration configuration, ILogger<JwtUtil> logger)
    {
        _logger = logger;

        _accessTokenSecret = configuration["Jwt:AccessTokenSecret"];
        _refreshTokenSecret = configuration["Jwt:RefreshTokenSecret"];
        _accessTokenExpiration = int.Parse(configuration["Jwt:AccessTokenExpiration"]);
        _refreshTokenExpiration = int.Parse(configuration["Jwt:RefreshTokenExpiration"]);

        _logger.LogInformation("JWT Configuration Loaded: AccessTokenExpiration={AccessTokenExpiration}, RefreshTokenExpiration={RefreshTokenExpiration}", _accessTokenExpiration, _refreshTokenExpiration);
    }

    public string GenerateAccessToken(string username, Guid userId, string role)
    {
        _logger.LogInformation("Generating access token for user: {Username}, Role: {Role}", username, role);
        
        var claims = new[]
        {
            new Claim("username", username),
            new Claim("id", userId.ToString()),
            new Claim("role", role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_accessTokenSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            expires: DateTime.UtcNow.AddMinutes(_accessTokenExpiration),
            claims: claims,
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        _logger.LogInformation("Access token generated successfully");
        return tokenString;
    }

    public string GenerateRefreshToken(string username)
    {
        _logger.LogInformation("Generating refresh token for user: {Username}", username);
        
        var claims = new[]
        {
            new Claim("username", username)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_refreshTokenSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            expires: DateTime.UtcNow.AddDays(_refreshTokenExpiration),
            claims: claims,
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        _logger.LogInformation("Refresh token generated successfully");
        return tokenString;
    }

    public ClaimsPrincipal ValidateRefreshToken(string token)
    {
        try
        {
            _logger.LogInformation("Validating refresh token");
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_refreshTokenSecret);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            _logger.LogInformation("Refresh token validated successfully");
            return principal;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to validate refresh token: {Message}", ex.Message);
            return null;
        }
    }

    public ClaimsPrincipal ValidateAccessToken(string token)
    {
        try
        {
            _logger.LogInformation("Validating access token");
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_accessTokenSecret);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            _logger.LogInformation("Access token validated successfully");
            return principal;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to validate access token: {Message}", ex.Message);
            return null;
        }
    }
}
