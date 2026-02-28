using System.ComponentModel.DataAnnotations;

namespace FinancePlatform.Application.DTOs.Categories;

public class UpdateCategoryRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Icon { get; set; }

    public string? Color { get; set; }
}
