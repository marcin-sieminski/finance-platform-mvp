using FinancePlatform.Domain.Entities;
using FinancePlatform.Domain.Enums;
using FinancePlatform.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinancePlatform.Infrastructure.Persistence.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _db;

    public CategoryRepository(AppDbContext db) => _db = db;

    public Task<Category?> GetByIdAsync(Guid id, Guid userId) =>
        _db.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

    public async Task<IEnumerable<Category>> GetAllByUserAsync(Guid userId, CategoryType? type = null)
    {
        var query = _db.Categories.Where(c => c.UserId == userId);
        if (type.HasValue)
            query = query.Where(c => c.Type == type.Value);
        return await query.OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<Category> CreateAsync(Category category)
    {
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();
        return category;
    }

    public async Task<Category> UpdateAsync(Category category)
    {
        _db.Categories.Update(category);
        await _db.SaveChangesAsync();
        return category;
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var cat = await GetByIdAsync(id, userId);
        if (cat != null)
        {
            _db.Categories.Remove(cat);
            await _db.SaveChangesAsync();
        }
    }

    public async Task CreateRangeAsync(IEnumerable<Category> categories)
    {
        _db.Categories.AddRange(categories);
        await _db.SaveChangesAsync();
    }
}
