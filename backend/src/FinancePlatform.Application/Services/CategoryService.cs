using FinancePlatform.Application.DTOs.Categories;
using FinancePlatform.Application.Interfaces;
using FinancePlatform.Domain.Entities;
using FinancePlatform.Domain.Enums;
using FinancePlatform.Domain.Interfaces;

namespace FinancePlatform.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categories;

    public CategoryService(ICategoryRepository categories)
    {
        _categories = categories;
    }

    public async Task<IEnumerable<CategoryResponse>> GetAllAsync(Guid userId, CategoryType? type = null)
    {
        var cats = await _categories.GetAllByUserAsync(userId, type);
        return cats.Select(MapToResponse);
    }

    public async Task<CategoryResponse> GetByIdAsync(Guid id, Guid userId)
    {
        var cat = await _categories.GetByIdAsync(id, userId)
            ?? throw new KeyNotFoundException($"Category {id} not found.");
        return MapToResponse(cat);
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, Guid userId)
    {
        var cat = new Category
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = request.Name,
            Type = request.Type,
            Icon = request.Icon,
            Color = request.Color,
            IsDefault = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _categories.CreateAsync(cat);
        return MapToResponse(created);
    }

    public async Task<CategoryResponse> UpdateAsync(Guid id, UpdateCategoryRequest request, Guid userId)
    {
        var cat = await _categories.GetByIdAsync(id, userId)
            ?? throw new KeyNotFoundException($"Category {id} not found.");

        if (cat.IsDefault)
            throw new InvalidOperationException("Default categories cannot be renamed via this endpoint.");

        cat.Name = request.Name;
        cat.Icon = request.Icon;
        cat.Color = request.Color;
        cat.UpdatedAt = DateTime.UtcNow;

        var updated = await _categories.UpdateAsync(cat);
        return MapToResponse(updated);
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var cat = await _categories.GetByIdAsync(id, userId)
            ?? throw new KeyNotFoundException($"Category {id} not found.");

        if (cat.IsDefault)
            throw new InvalidOperationException("Default categories cannot be deleted.");

        await _categories.DeleteAsync(id, userId);
    }

    private static CategoryResponse MapToResponse(Category c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Type = c.Type,
        Icon = c.Icon,
        Color = c.Color,
        IsDefault = c.IsDefault
    };
}
