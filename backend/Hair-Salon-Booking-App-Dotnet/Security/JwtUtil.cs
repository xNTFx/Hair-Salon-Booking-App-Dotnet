using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;

public class JwtUtil
{
    private readonly string _accessTokenSecret;
    private readonly string _refreshTokenSecret;
    private readonly int _accessTokenExpiration;
    private readonly int _refreshTokenExpiration;

    public JwtUtil(IConfiguration configuration)
    {
        _accessTokenSecret = configuration["Jwt:AccessTokenSecret"];
        _refreshTokenSecret = configuration["Jwt:RefreshTokenSecret"];
        _accessTokenExpiration = int.Parse(configuration["Jwt:AccessTokenExpiration"]);
        _refreshTokenExpiration = int.Parse(configuration["Jwt:RefreshTokenExpiration"]);
    }

    public string GenerateAccessToken(string username, Guid userId, string role)
    {
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

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken(string username)
    {
        var claims = new[]
        {
        new Claim("username", username),
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_refreshTokenSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            expires: DateTime.UtcNow.AddDays(_refreshTokenExpiration),
            claims: claims,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    public ClaimsPrincipal ValidateRefreshToken(string token)
    {
        try
        {
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
            return principal;
        }
        catch
        {
            return null;
        }
    }

    public ClaimsPrincipal ValidateAccessToken(string token)
    {
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

        return tokenHandler.ValidateToken(token, validationParameters, out _);
    }
}
