using FinancePlatform.Application.DTOs.Dashboard;
using FinancePlatform.Application.DTOs.Transactions;
using FinancePlatform.Application.Interfaces;
using FinancePlatform.Domain.Enums;
using FinancePlatform.Domain.Interfaces;

namespace FinancePlatform.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IAccountRepository _accounts;
    private readonly ITransactionRepository _transactions;

    public DashboardService(IAccountRepository accounts, ITransactionRepository transactions)
    {
        _accounts = accounts;
        _transactions = transactions;
    }

    public async Task<DashboardSummaryResponse> GetSummaryAsync(Guid userId)
    {
        var now = DateTime.UtcNow;

        var accounts = await _accounts.GetAllByUserAsync(userId);
        var totalBalance = accounts.Where(a => !a.IsArchived).Sum(a => a.Balance);

        var monthlyIncome = await _transactions.GetTotalByTypeAndPeriodAsync(userId, TransactionType.Income, now.Month, now.Year);
        var monthlyExpenses = await _transactions.GetTotalByTypeAndPeriodAsync(userId, TransactionType.Expense, now.Month, now.Year);

        var flowData = await _transactions.GetMonthlyFlowAsync(userId, 6);
        var categoryData = await _transactions.GetExpensesByCategoryAsync(userId, now.Month, now.Year);
        var recent = await _transactions.GetRecentByUserAsync(userId, 5);

        var totalExpenses = categoryData.Sum(c => c.Amount);
        var expensesByCategory = categoryData.Select(c => new CategoryBreakdownItem
        {
            CategoryId = c.CategoryId,
            Name = c.CategoryName,
            Color = c.CategoryColor,
            Amount = c.Amount,
            Percent = totalExpenses > 0 ? Math.Round(c.Amount / totalExpenses * 100, 1) : 0
        });

        var monthlyFlow = flowData.Select(f => new MonthlyFlowDataPoint
        {
            Month = new DateTime(f.Year, f.Month, 1).ToString("MMM"),
            MonthNumber = f.Month,
            Year = f.Year,
            Income = f.Income,
            Expenses = f.Expenses
        });

        return new DashboardSummaryResponse
        {
            TotalBalance = totalBalance,
            MonthlyIncome = monthlyIncome,
            MonthlyExpenses = monthlyExpenses,
            MonthlySavings = monthlyIncome - monthlyExpenses,
            Last6MonthsFlow = monthlyFlow,
            ExpensesByCategory = expensesByCategory,
            RecentTransactions = recent.Select(t => new TransactionResponse
            {
                Id = t.Id,
                AccountId = t.AccountId,
                AccountName = t.Account?.Name ?? string.Empty,
                CategoryId = t.CategoryId,
                CategoryName = t.Category?.Name ?? string.Empty,
                CategoryColor = t.Category?.Color,
                CategoryIcon = t.Category?.Icon,
                Amount = t.Amount,
                Type = t.Type,
                Description = t.Description,
                Date = t.Date,
                CreatedAt = t.CreatedAt
            })
        };
    }
}
