using FinancePlatform.Domain.Enums;

namespace FinancePlatform.Application.DTOs.Accounts;

public class AccountResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public AccountType Type { get; set; }
    public decimal Balance { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string? Color { get; set; }
    public bool IsArchived { get; set; }
    public DateTime CreatedAt { get; set; }
}
