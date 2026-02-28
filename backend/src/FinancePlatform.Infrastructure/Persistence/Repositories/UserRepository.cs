using FinancePlatform.Domain.Entities;
using FinancePlatform.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinancePlatform.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db) => _db = db;

    public Task<User?> GetByIdAsync(Guid id) =>
        _db.Users.FirstOrDefaultAsync(u => u.Id == id);

    public Task<User?> GetByEmailAsync(string email) =>
        _db.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User> CreateAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public Task<bool> ExistsAsync(string email) =>
        _db.Users.AnyAsync(u => u.Email == email.ToLowerInvariant());
}
