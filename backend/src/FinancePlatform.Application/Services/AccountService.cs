using FinancePlatform.Application.DTOs.Accounts;
using FinancePlatform.Application.Interfaces;
using FinancePlatform.Domain.Entities;
using FinancePlatform.Domain.Interfaces;

namespace FinancePlatform.Application.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accounts;

    public AccountService(IAccountRepository accounts)
    {
        _accounts = accounts;
    }

    public async Task<IEnumerable<AccountResponse>> GetAllAsync(Guid userId)
    {
        var accounts = await _accounts.GetAllByUserAsync(userId);
        return accounts.Select(MapToResponse);
    }

    public async Task<AccountResponse> GetByIdAsync(Guid id, Guid userId)
    {
        var account = await _accounts.GetByIdAsync(id, userId)
            ?? throw new KeyNotFoundException($"Account {id} not found.");
        return MapToResponse(account);
    }

    public async Task<AccountResponse> CreateAsync(CreateAccountRequest request, Guid userId)
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = request.Name,
            Type = request.Type,
            Balance = request.Balance,
            Currency = request.Currency,
            Color = request.Color,
            IsArchived = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _accounts.CreateAsync(account);
        return MapToResponse(created);
    }

    public async Task<AccountResponse> UpdateAsync(Guid id, UpdateAccountRequest request, Guid userId)
    {
        var account = await _accounts.GetByIdAsync(id, userId)
            ?? throw new KeyNotFoundException($"Account {id} not found.");

        account.Name = request.Name;
        account.Type = request.Type;
        account.Currency = request.Currency;
        account.Color = request.Color;
        account.UpdatedAt = DateTime.UtcNow;

        var updated = await _accounts.UpdateAsync(account);
        return MapToResponse(updated);
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var account = await _accounts.GetByIdAsync(id, userId)
            ?? throw new KeyNotFoundException($"Account {id} not found.");

        account.IsArchived = true;
        account.UpdatedAt = DateTime.UtcNow;
        await _accounts.UpdateAsync(account);
    }

    private static AccountResponse MapToResponse(Account a) => new()
    {
        Id = a.Id,
        Name = a.Name,
        Type = a.Type,
        Balance = a.Balance,
        Currency = a.Currency,
        Color = a.Color,
        IsArchived = a.IsArchived,
        CreatedAt = a.CreatedAt
    };
}
