using FinancePlatform.Application.DTOs.Transactions;

namespace FinancePlatform.Application.DTOs.Dashboard;

public class DashboardSummaryResponse
{
    public decimal TotalBalance { get; set; }
    public decimal MonthlyIncome { get; set; }
    public decimal MonthlyExpenses { get; set; }
    public decimal MonthlySavings { get; set; }
    public IEnumerable<MonthlyFlowDataPoint> Last6MonthsFlow { get; set; } = Enumerable.Empty<MonthlyFlowDataPoint>();
    public IEnumerable<CategoryBreakdownItem> ExpensesByCategory { get; set; } = Enumerable.Empty<CategoryBreakdownItem>();
    public IEnumerable<TransactionResponse> RecentTransactions { get; set; } = Enumerable.Empty<TransactionResponse>();
}

public class MonthlyFlowDataPoint
{
    public string Month { get; set; } = string.Empty;
    public int MonthNumber { get; set; }
    public int Year { get; set; }
    public decimal Income { get; set; }
    public decimal Expenses { get; set; }
}

public class CategoryBreakdownItem
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
    public decimal Amount { get; set; }
    public decimal Percent { get; set; }
}
