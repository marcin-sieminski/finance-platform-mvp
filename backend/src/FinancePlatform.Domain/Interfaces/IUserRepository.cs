using FinancePlatform.Domain.Entities;

namespace FinancePlatform.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User> CreateAsync(User user);
    Task<bool> ExistsAsync(string email);
}
