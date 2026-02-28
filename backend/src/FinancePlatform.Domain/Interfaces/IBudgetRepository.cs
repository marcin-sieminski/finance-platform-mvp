using FinancePlatform.Domain.Entities;

namespace FinancePlatform.Domain.Interfaces;

public interface IBudgetRepository
{
    Task<Budget?> GetByIdAsync(Guid id, Guid userId);
    Task<IEnumerable<Budget>> GetByPeriodAsync(Guid userId, int month, int year);
    Task<Budget> CreateAsync(Budget budget);
    Task<Budget> UpdateAsync(Budget budget);
    Task DeleteAsync(Guid id, Guid userId);
    Task<bool> ExistsAsync(Guid userId, Guid categoryId, int month, int year);
}
