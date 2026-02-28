using FinancePlatform.Application.DTOs.Categories;
using FinancePlatform.Domain.Enums;

namespace FinancePlatform.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryResponse>> GetAllAsync(Guid userId, CategoryType? type = null);
    Task<CategoryResponse> GetByIdAsync(Guid id, Guid userId);
    Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, Guid userId);
    Task<CategoryResponse> UpdateAsync(Guid id, UpdateCategoryRequest request, Guid userId);
    Task DeleteAsync(Guid id, Guid userId);
}
