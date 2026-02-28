using FinancePlatform.Domain.Enums;

namespace FinancePlatform.Application.DTOs.Categories;

public class CategoryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public CategoryType Type { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public bool IsDefault { get; set; }
}
