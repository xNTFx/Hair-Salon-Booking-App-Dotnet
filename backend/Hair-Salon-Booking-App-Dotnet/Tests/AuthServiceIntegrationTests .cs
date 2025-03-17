using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using System.Transactions;

public class AuthServiceIntegrationTests : IDisposable
{
    private readonly AuthService _authService;
    private readonly UserRepository _userRepository;
    private readonly AppDbContext _dbContext;
    private readonly TransactionScope _transactionScope;

    public AuthServiceIntegrationTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        _dbContext = new AppDbContext(options);
        _userRepository = new UserRepository(_dbContext);
        _authService = new AuthService(_userRepository);
        _transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldCreateUser_WhenValidDataProvided()
    {
        Console.WriteLine("Testing if CreateUserAsync correctly creates a new user.");

        var user = await _authService.CreateUserAsync("testuser", "password123", "user");

        Assert.NotNull(user);
        Assert.Equal("testuser", user.Username);
        Assert.NotNull(user.Password);
    }

    [Fact]
    public async Task FindUserByUsernameAsync_ShouldReturnUser_WhenExists()
    {
        Console.WriteLine("Testing if FindUserByUsernameAsync returns a user when it exists.");

        var createdUser = await _authService.CreateUserAsync("existinguser", "password123", "user");
        var foundUser = await _authService.FindUserByUsernameAsync("existinguser");

        Assert.NotNull(foundUser);
        Assert.Equal(createdUser.Id, foundUser.Id);
        Assert.Equal("existinguser", foundUser.Username);
    }

    [Fact]
    public async Task FindUserByUsernameAsync_ShouldReturnNull_WhenNotExists()
    {
        Console.WriteLine("Testing if FindUserByUsernameAsync returns null when the user does not exist.");

        var user = await _authService.FindUserByUsernameAsync("nonexistent");

        Assert.Null(user);
    }

    [Fact]
    public async Task CheckDuplicateUsernameAsync_ShouldReturnTrue_WhenUsernameExists()
    {
        Console.WriteLine("Testing if CheckDuplicateUsernameAsync returns true when a username already exists.");

        await _authService.CreateUserAsync("duplicateuser", "password123", "user");
        var exists = await _authService.CheckDuplicateUsernameAsync("duplicateuser");

        Assert.True(exists);
    }

    [Fact]
    public async Task CheckDuplicateUsernameAsync_ShouldReturnFalse_WhenUsernameDoesNotExist()
    {
        Console.WriteLine("Testing if CheckDuplicateUsernameAsync returns false when a username does not exist.");

        var exists = await _authService.CheckDuplicateUsernameAsync("uniqueuser");

        Assert.False(exists);
    }

    [Fact]
    public async Task UpdateRefreshTokenAsync_ShouldUpdateToken_WhenUserExists()
    {
        Console.WriteLine("Testing if UpdateRefreshTokenAsync correctly updates the refresh token.");

        var user = await _authService.CreateUserAsync("tokenuser", "password123", "user");
        string newRefreshToken = Guid.NewGuid().ToString();

        await _authService.UpdateRefreshTokenAsync(user.Id, newRefreshToken);
        var updatedUser = await _authService.FindUserByUsernameAsync("tokenuser");

        Assert.NotNull(updatedUser);
        Assert.Equal(newRefreshToken, updatedUser.RefreshToken);
    }

    [Fact]
    public async Task FindUserByRefreshTokenAsync_ShouldReturnUser_WhenTokenExists()
    {
        Console.WriteLine("Testing if FindUserByRefreshTokenAsync returns the correct user based on refresh token.");

        var user = await _authService.CreateUserAsync("refreshuser", "password123", "user");
        string refreshToken = Guid.NewGuid().ToString();

        await _authService.UpdateRefreshTokenAsync(user.Id, refreshToken);
        var foundUser = await _authService.FindUserByRefreshTokenAsync(refreshToken);

        Assert.NotNull(foundUser);
        Assert.Equal(user.Id, foundUser.Id);
    }

    public void Dispose()
    {
        _transactionScope.Dispose();
        _dbContext.Dispose();
    }
}