using FinancePlatform.Domain.Entities;
using FinancePlatform.Domain.Enums;

namespace FinancePlatform.Domain.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(Guid id, Guid userId);
    Task<(IEnumerable<Transaction> Items, int TotalCount)> GetPagedByUserAsync(
        Guid userId, int page, int pageSize,
        Guid? accountId, Guid? categoryId,
        TransactionType? type, DateTime? from, DateTime? to);
    Task<IEnumerable<Transaction>> GetRecentByUserAsync(Guid userId, int count);
    Task<decimal> GetSumByCategoryAndPeriodAsync(Guid userId, Guid categoryId, int month, int year);
    Task<decimal> GetTotalByTypeAndPeriodAsync(Guid userId, TransactionType type, int month, int year);
    Task<IEnumerable<(int Month, int Year, decimal Income, decimal Expenses)>> GetMonthlyFlowAsync(Guid userId, int months);
    Task<IEnumerable<(Guid CategoryId, string CategoryName, string? CategoryColor, decimal Amount)>> GetExpensesByCategoryAsync(Guid userId, int month, int year);
    Task<Transaction> CreateAsync(Transaction transaction);
    Task<Transaction> UpdateAsync(Transaction transaction);
    Task DeleteAsync(Guid id, Guid userId);
}
