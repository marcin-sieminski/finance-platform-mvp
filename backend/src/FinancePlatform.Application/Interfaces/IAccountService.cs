using FinancePlatform.Application.DTOs.Accounts;

namespace FinancePlatform.Application.Interfaces;

public interface IAccountService
{
    Task<IEnumerable<AccountResponse>> GetAllAsync(Guid userId);
    Task<AccountResponse> GetByIdAsync(Guid id, Guid userId);
    Task<AccountResponse> CreateAsync(CreateAccountRequest request, Guid userId);
    Task<AccountResponse> UpdateAsync(Guid id, UpdateAccountRequest request, Guid userId);
    Task DeleteAsync(Guid id, Guid userId);
}
