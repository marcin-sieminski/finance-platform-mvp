using FinancePlatform.Application.DTOs.Transactions;
using FinancePlatform.Application.Interfaces;
using FinancePlatform.Domain.Entities;
using FinancePlatform.Domain.Interfaces;

namespace FinancePlatform.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactions;
    private readonly IAccountRepository _accounts;

    public TransactionService(ITransactionRepository transactions, IAccountRepository accounts)
    {
        _transactions = transactions;
        _accounts = accounts;
    }

    public async Task<PagedResult<TransactionResponse>> GetPagedAsync(TransactionQueryParams query, Guid userId)
    {
        var pageSize = Math.Min(query.PageSize, 100);
        var (items, total) = await _transactions.GetPagedByUserAsync(
            userId, query.Page, pageSize,
            query.AccountId, query.CategoryId,
            query.Type, query.From, query.To);

        return new PagedResult<TransactionResponse>
        {
            Items = items.Select(MapToResponse),
            TotalCount = total,
            Page = query.Page,
            PageSize = pageSize
        };
    }

    public async Task<TransactionResponse> GetByIdAsync(Guid id, Guid userId)
    {
        var t = await _transactions.GetByIdAsync(id, userId)
            ?? throw new KeyNotFoundException($"Transaction {id} not found.");
        return MapToResponse(t);
    }

    public async Task<TransactionResponse> CreateAsync(CreateTransactionRequest request, Guid userId)
    {
        var account = await _accounts.GetByIdAsync(request.AccountId, userId)
            ?? throw new KeyNotFoundException($"Account {request.AccountId} not found.");

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            AccountId = request.AccountId,
            CategoryId = request.CategoryId,
            Amount = request.Amount,
            Type = request.Type,
            Description = request.Description,
            Date = request.Date,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        ApplyBalanceChange(account, request.Type, request.Amount);
        account.UpdatedAt = DateTime.UtcNow;

        await _accounts.UpdateAsync(account);
        var created = await _transactions.CreateAsync(transaction);

        // Reload with navigation properties
        var result = await _transactions.GetByIdAsync(created.Id, userId);
        return MapToResponse(result!);
    }

    public async Task<TransactionResponse> UpdateAsync(Guid id, UpdateTransactionRequest request, Guid userId)
    {
        var existing = await _transactions.GetByIdAsync(id, userId)
            ?? throw new KeyNotFoundException($"Transaction {id} not found.");

        // Reverse old effect
        var oldAccount = await _accounts.GetByIdAsync(existing.AccountId, userId)!;
        if (oldAccount != null)
        {
            ReverseBalanceChange(oldAccount, existing.Type, existing.Amount);
            oldAccount.UpdatedAt = DateTime.UtcNow;
            await _accounts.UpdateAsync(oldAccount);
        }

        // Apply new effect (possibly to a different account)
        var newAccount = await _accounts.GetByIdAsync(request.AccountId, userId)
            ?? throw new KeyNotFoundException($"Account {request.AccountId} not found.");

        ApplyBalanceChange(newAccount, request.Type, request.Amount);
        newAccount.UpdatedAt = DateTime.UtcNow;
        await _accounts.UpdateAsync(newAccount);

        existing.AccountId = request.AccountId;
        existing.CategoryId = request.CategoryId;
        existing.Amount = request.Amount;
        existing.Type = request.Type;
        existing.Description = request.Description;
        existing.Date = request.Date;
        existing.UpdatedAt = DateTime.UtcNow;

        var updated = await _transactions.UpdateAsync(existing);
        var result = await _transactions.GetByIdAsync(updated.Id, userId);
        return MapToResponse(result!);
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var existing = await _transactions.GetByIdAsync(id, userId)
            ?? throw new KeyNotFoundException($"Transaction {id} not found.");

        var account = await _accounts.GetByIdAsync(existing.AccountId, userId);
        if (account != null)
        {
            ReverseBalanceChange(account, existing.Type, existing.Amount);
            account.UpdatedAt = DateTime.UtcNow;
            await _accounts.UpdateAsync(account);
        }

        await _transactions.DeleteAsync(id, userId);
    }

    private static void ApplyBalanceChange(Domain.Entities.Account account, Domain.Enums.TransactionType type, decimal amount)
    {
        if (type == Domain.Enums.TransactionType.Income)
            account.Balance += amount;
        else
            account.Balance -= amount;
    }

    private static void ReverseBalanceChange(Domain.Entities.Account account, Domain.Enums.TransactionType type, decimal amount)
    {
        if (type == Domain.Enums.TransactionType.Income)
            account.Balance -= amount;
        else
            account.Balance += amount;
    }

    private static TransactionResponse MapToResponse(Transaction t) => new()
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
    };
}
