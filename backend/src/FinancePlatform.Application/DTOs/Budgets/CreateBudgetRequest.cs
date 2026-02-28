using System.ComponentModel.DataAnnotations;

namespace FinancePlatform.Application.DTOs.Budgets;

public class CreateBudgetRequest
{
    [Required]
    public Guid CategoryId { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Limit must be positive")]
    public decimal LimitAmount { get; set; }

    [Range(1, 12)]
    public int Month { get; set; }

    [Range(2000, 9999)]
    public int Year { get; set; }
}
