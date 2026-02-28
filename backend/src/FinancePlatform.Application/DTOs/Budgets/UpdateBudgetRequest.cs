using System.ComponentModel.DataAnnotations;

namespace FinancePlatform.Application.DTOs.Budgets;

public class UpdateBudgetRequest
{
    [Range(0.01, double.MaxValue, ErrorMessage = "Limit must be positive")]
    public decimal LimitAmount { get; set; }
}
