public interface IUserRepository
{
    Task<User> FindByUsernameAsync(string username);
    Task<User> FindByRefreshTokenAsync(string refreshToken);
    Task<bool> ExistsByUsernameAsync(string username);
    Task<User> SaveUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task<User> FindByIdAsync(Guid userId);
}
