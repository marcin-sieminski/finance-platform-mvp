using System.ComponentModel.DataAnnotations;
using FinancePlatform.Domain.Enums;

namespace FinancePlatform.Application.DTOs.Categories;

public class CreateCategoryRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public CategoryType Type { get; set; }

    public string? Icon { get; set; }

    public string? Color { get; set; }
}
