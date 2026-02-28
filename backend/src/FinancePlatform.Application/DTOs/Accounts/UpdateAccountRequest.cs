using System.ComponentModel.DataAnnotations;
using FinancePlatform.Domain.Enums;

namespace FinancePlatform.Application.DTOs.Accounts;

public class UpdateAccountRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public AccountType Type { get; set; }

    [MaxLength(3)]
    public string Currency { get; set; } = "USD";

    public string? Color { get; set; }
}
