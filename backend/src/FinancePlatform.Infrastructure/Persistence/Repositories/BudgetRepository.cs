using FinancePlatform.Domain.Entities;
using FinancePlatform.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinancePlatform.Infrastructure.Persistence.Repositories;

public class BudgetRepository : IBudgetRepository
{
    private readonly AppDbContext _db;

    public BudgetRepository(AppDbContext db) => _db = db;

    public Task<Budget?> GetByIdAsync(Guid id, Guid userId) =>
        _db.Budgets
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

    public async Task<IEnumerable<Budget>> GetByPeriodAsync(Guid userId, int month, int year) =>
        await _db.Budgets
            .Include(b => b.Category)
            .Where(b => b.UserId == userId && b.Month == month && b.Year == year)
            .OrderBy(b => b.Category.Name)
            .ToListAsync();

    public async Task<Budget> CreateAsync(Budget budget)
    {
        _db.Budgets.Add(budget);
        await _db.SaveChangesAsync();
        return budget;
    }

    public async Task<Budget> UpdateAsync(Budget budget)
    {
        _db.Budgets.Update(budget);
        await _db.SaveChangesAsync();
        return budget;
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var b = await _db.Budgets.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
        if (b != null)
        {
            _db.Budgets.Remove(b);
            await _db.SaveChangesAsync();
        }
    }

    public Task<bool> ExistsAsync(Guid userId, Guid categoryId, int month, int year) =>
        _db.Budgets.AnyAsync(b => b.UserId == userId
            && b.CategoryId == categoryId
            && b.Month == month
            && b.Year == year);
}
