using FinancePlatform.Domain.Enums;

namespace FinancePlatform.Application.DTOs.Transactions;

public class TransactionQueryParams
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public Guid? AccountId { get; set; }
    public Guid? CategoryId { get; set; }
    public TransactionType? Type { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}
