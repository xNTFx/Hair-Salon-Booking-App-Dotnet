using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly PasswordHasher<User> _passwordHasher;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
        _passwordHasher = new PasswordHasher<User>();
    }

    public async Task<User> FindUserByUsernameAsync(string username)
    {
        return await _userRepository.FindByUsernameAsync(username);
    }

    public async Task<User> FindUserByRefreshTokenAsync(string refreshToken)
    {
        return await _userRepository.FindByRefreshTokenAsync(refreshToken);
    }

    public async Task UpdateRefreshTokenAsync(Guid userId, string refreshToken)
    {
        var user = await _userRepository.FindByIdAsync(userId);
        if (user != null)
        {
            user.RefreshToken = refreshToken;
            await _userRepository.UpdateUserAsync(user);
        }
    }


    public async Task<User> CreateUserAsync(string username, string password, string role)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            Password = _passwordHasher.HashPassword(null, password),
            Role = role
        };

        return await _userRepository.SaveUserAsync(user);
    }

    public async Task<bool> CheckDuplicateUsernameAsync(string username)
    {
        return await _userRepository.ExistsByUsernameAsync(username);
    }
}
