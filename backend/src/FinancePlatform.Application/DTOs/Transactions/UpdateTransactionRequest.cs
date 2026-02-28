using System.ComponentModel.DataAnnotations;
using FinancePlatform.Domain.Enums;

namespace FinancePlatform.Application.DTOs.Transactions;

public class UpdateTransactionRequest
{
    [Required]
    public Guid AccountId { get; set; }

    [Required]
    public Guid CategoryId { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be positive")]
    public decimal Amount { get; set; }

    public TransactionType Type { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public DateTime Date { get; set; }
}
