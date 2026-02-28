using FinancePlatform.Application.DTOs.Budgets;
using FinancePlatform.Application.Interfaces;
using FinancePlatform.Domain.Entities;
using FinancePlatform.Domain.Interfaces;

namespace FinancePlatform.Application.Services;

public class BudgetService : IBudgetService
{
    private readonly IBudgetRepository _budgets;
    private readonly ITransactionRepository _transactions;

    public BudgetService(IBudgetRepository budgets, ITransactionRepository transactions)
    {
        _budgets = budgets;
        _transactions = transactions;
    }

    public async Task<IEnumerable<BudgetResponse>> GetByPeriodAsync(Guid userId, int month, int year)
    {
        var budgets = await _budgets.GetByPeriodAsync(userId, month, year);
        var responses = new List<BudgetResponse>();

        foreach (var b in budgets)
        {
            var spent = await _transactions.GetSumByCategoryAndPeriodAsync(userId, b.CategoryId, month, year);
            responses.Add(MapToResponse(b, spent));
        }

        return responses;
    }

    public async Task<BudgetResponse> GetByIdAsync(Guid id, Guid userId)
    {
        var budget = await _budgets.GetByIdAsync(id, userId)
            ?? throw new KeyNotFoundException($"Budget {id} not found.");

        var spent = await _transactions.GetSumByCategoryAndPeriodAsync(userId, budget.CategoryId, budget.Month, budget.Year);
        return MapToResponse(budget, spent);
    }

    public async Task<BudgetResponse> CreateAsync(CreateBudgetRequest request, Guid userId)
    {
        if (await _budgets.ExistsAsync(userId, request.CategoryId, request.Month, request.Year))
            throw new InvalidOperationException("A budget for this category and period already exists.");

        var budget = new Budget
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CategoryId = request.CategoryId,
            LimitAmount = request.LimitAmount,
            Month = request.Month,
            Year = request.Year,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _budgets.CreateAsync(budget);
        var spent = await _transactions.GetSumByCategoryAndPeriodAsync(userId, created.CategoryId, created.Month, created.Year);
        var result = await _budgets.GetByIdAsync(created.Id, userId);
        return MapToResponse(result!, spent);
    }

    public async Task<BudgetResponse> UpdateAsync(Guid id, UpdateBudgetRequest request, Guid userId)
    {
        var budget = await _budgets.GetByIdAsync(id, userId)
            ?? throw new KeyNotFoundException($"Budget {id} not found.");

        budget.LimitAmount = request.LimitAmount;
        budget.UpdatedAt = DateTime.UtcNow;

        var updated = await _budgets.UpdateAsync(budget);
        var spent = await _transactions.GetSumByCategoryAndPeriodAsync(userId, updated.CategoryId, updated.Month, updated.Year);
        var result = await _budgets.GetByIdAsync(updated.Id, userId);
        return MapToResponse(result!, spent);
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        _ = await _budgets.GetByIdAsync(id, userId)
            ?? throw new KeyNotFoundException($"Budget {id} not found.");

        await _budgets.DeleteAsync(id, userId);
    }

    private static BudgetResponse MapToResponse(Budget b, decimal spent) => new()
    {
        Id = b.Id,
        CategoryId = b.CategoryId,
        CategoryName = b.Category?.Name ?? string.Empty,
        CategoryColor = b.Category?.Color,
        CategoryIcon = b.Category?.Icon,
        LimitAmount = b.LimitAmount,
        SpentAmount = spent,
        Month = b.Month,
        Year = b.Year
    };
}
