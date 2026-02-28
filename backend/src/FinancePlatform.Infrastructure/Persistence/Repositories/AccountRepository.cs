using FinancePlatform.Domain.Entities;
using FinancePlatform.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinancePlatform.Infrastructure.Persistence.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly AppDbContext _db;

    public AccountRepository(AppDbContext db) => _db = db;

    public Task<Account?> GetByIdAsync(Guid id, Guid userId) =>
        _db.Accounts.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

    public async Task<IEnumerable<Account>> GetAllByUserAsync(Guid userId) =>
        await _db.Accounts
            .Where(a => a.UserId == userId)
            .OrderBy(a => a.Name)
            .ToListAsync();

    public async Task<Account> CreateAsync(Account account)
    {
        _db.Accounts.Add(account);
        await _db.SaveChangesAsync();
        return account;
    }

    public async Task<Account> UpdateAsync(Account account)
    {
        _db.Accounts.Update(account);
        await _db.SaveChangesAsync();
        return account;
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var account = await GetByIdAsync(id, userId);
        if (account != null)
        {
            _db.Accounts.Remove(account);
            await _db.SaveChangesAsync();
        }
    }
}
