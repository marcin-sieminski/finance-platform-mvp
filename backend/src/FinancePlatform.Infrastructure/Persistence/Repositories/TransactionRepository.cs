using FinancePlatform.Domain.Entities;
using FinancePlatform.Domain.Enums;
using FinancePlatform.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinancePlatform.Infrastructure.Persistence.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _db;

    public TransactionRepository(AppDbContext db) => _db = db;

    public Task<Transaction?> GetByIdAsync(Guid id, Guid userId) =>
        _db.Transactions
            .Include(t => t.Account)
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

    public async Task<(IEnumerable<Transaction> Items, int TotalCount)> GetPagedByUserAsync(
        Guid userId, int page, int pageSize,
        Guid? accountId, Guid? categoryId,
        TransactionType? type, DateTime? from, DateTime? to)
    {
        var query = _db.Transactions
            .Include(t => t.Account)
            .Include(t => t.Category)
            .Where(t => t.UserId == userId);

        if (accountId.HasValue) query = query.Where(t => t.AccountId == accountId);
        if (categoryId.HasValue) query = query.Where(t => t.CategoryId == categoryId);
        if (type.HasValue) query = query.Where(t => t.Type == type);
        if (from.HasValue) query = query.Where(t => t.Date >= from);
        if (to.HasValue) query = query.Where(t => t.Date <= to);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<IEnumerable<Transaction>> GetRecentByUserAsync(Guid userId, int count) =>
        await _db.Transactions
            .Include(t => t.Account)
            .Include(t => t.Category)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.Date)
            .Take(count)
            .ToListAsync();

    public async Task<decimal> GetSumByCategoryAndPeriodAsync(Guid userId, Guid categoryId, int month, int year) =>
        await _db.Transactions
            .Where(t => t.UserId == userId
                && t.CategoryId == categoryId
                && t.Type == TransactionType.Expense
                && t.Date.Month == month
                && t.Date.Year == year)
            .SumAsync(t => (decimal?)t.Amount) ?? 0;

    public async Task<decimal> GetTotalByTypeAndPeriodAsync(Guid userId, TransactionType type, int month, int year) =>
        await _db.Transactions
            .Where(t => t.UserId == userId
                && t.Type == type
                && t.Date.Month == month
                && t.Date.Year == year)
            .SumAsync(t => (decimal?)t.Amount) ?? 0;

    public async Task<IEnumerable<(int Month, int Year, decimal Income, decimal Expenses)>> GetMonthlyFlowAsync(Guid userId, int months)
    {
        var from = DateTime.UtcNow.AddMonths(-(months - 1));
        var startDate = new DateTime(from.Year, from.Month, 1);

        var data = await _db.Transactions
            .Where(t => t.UserId == userId && t.Date >= startDate)
            .GroupBy(t => new { t.Date.Month, t.Date.Year, t.Type })
            .Select(g => new
            {
                g.Key.Month,
                g.Key.Year,
                g.Key.Type,
                Total = g.Sum(t => t.Amount)
            })
            .ToListAsync();

        var result = new List<(int Month, int Year, decimal Income, decimal Expenses)>();
        for (int i = months - 1; i >= 0; i--)
        {
            var date = DateTime.UtcNow.AddMonths(-i);
            var income = data.FirstOrDefault(d => d.Month == date.Month && d.Year == date.Year && d.Type == TransactionType.Income)?.Total ?? 0;
            var expenses = data.FirstOrDefault(d => d.Month == date.Month && d.Year == date.Year && d.Type == TransactionType.Expense)?.Total ?? 0;
            result.Add((date.Month, date.Year, income, expenses));
        }

        return result;
    }

    public async Task<IEnumerable<(Guid CategoryId, string CategoryName, string? CategoryColor, decimal Amount)>> GetExpensesByCategoryAsync(Guid userId, int month, int year)
    {
        var data = await _db.Transactions
            .Include(t => t.Category)
            .Where(t => t.UserId == userId
                && t.Type == TransactionType.Expense
                && t.Date.Month == month
                && t.Date.Year == year)
            .GroupBy(t => new { t.CategoryId, t.Category.Name, t.Category.Color })
            .Select(g => new
            {
                g.Key.CategoryId,
                g.Key.Name,
                g.Key.Color,
                Amount = g.Sum(t => t.Amount)
            })
            .OrderByDescending(g => g.Amount)
            .ToListAsync();

        return data.Select(d => (d.CategoryId, d.Name, d.Color, d.Amount));
    }

    public async Task<Transaction> CreateAsync(Transaction transaction)
    {
        _db.Transactions.Add(transaction);
        await _db.SaveChangesAsync();
        return transaction;
    }

    public async Task<Transaction> UpdateAsync(Transaction transaction)
    {
        _db.Transactions.Update(transaction);
        await _db.SaveChangesAsync();
        return transaction;
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var t = await _db.Transactions.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (t != null)
        {
            _db.Transactions.Remove(t);
            await _db.SaveChangesAsync();
        }
    }
}
