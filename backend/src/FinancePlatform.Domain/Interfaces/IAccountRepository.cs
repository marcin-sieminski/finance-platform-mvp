using FinancePlatform.Domain.Entities;

namespace FinancePlatform.Domain.Interfaces;

public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(Guid id, Guid userId);
    Task<IEnumerable<Account>> GetAllByUserAsync(Guid userId);
    Task<Account> CreateAsync(Account account);
    Task<Account> UpdateAsync(Account account);
    Task DeleteAsync(Guid id, Guid userId);
}
