namespace FinancePlatform.Application.DTOs.Budgets;

public class BudgetResponse
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? CategoryColor { get; set; }
    public string? CategoryIcon { get; set; }
    public decimal LimitAmount { get; set; }
    public decimal SpentAmount { get; set; }
    public decimal RemainingAmount => LimitAmount - SpentAmount;
    public decimal ProgressPercent => LimitAmount > 0 ? Math.Min(SpentAmount / LimitAmount * 100, 100) : 0;
    public int Month { get; set; }
    public int Year { get; set; }
}
