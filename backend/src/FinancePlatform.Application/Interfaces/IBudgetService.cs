using FinancePlatform.Application.DTOs.Budgets;

namespace FinancePlatform.Application.Interfaces;

public interface IBudgetService
{
    Task<IEnumerable<BudgetResponse>> GetByPeriodAsync(Guid userId, int month, int year);
    Task<BudgetResponse> GetByIdAsync(Guid id, Guid userId);
    Task<BudgetResponse> CreateAsync(CreateBudgetRequest request, Guid userId);
    Task<BudgetResponse> UpdateAsync(Guid id, UpdateBudgetRequest request, Guid userId);
    Task DeleteAsync(Guid id, Guid userId);
}
