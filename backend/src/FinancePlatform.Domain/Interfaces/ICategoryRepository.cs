using FinancePlatform.Domain.Entities;
using FinancePlatform.Domain.Enums;

namespace FinancePlatform.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id, Guid userId);
    Task<IEnumerable<Category>> GetAllByUserAsync(Guid userId, CategoryType? type = null);
    Task<Category> CreateAsync(Category category);
    Task<Category> UpdateAsync(Category category);
    Task DeleteAsync(Guid id, Guid userId);
    Task CreateRangeAsync(IEnumerable<Category> categories);
}
