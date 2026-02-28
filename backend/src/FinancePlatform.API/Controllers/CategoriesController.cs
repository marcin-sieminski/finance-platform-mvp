using FinancePlatform.Application.DTOs.Categories;
using FinancePlatform.Application.Interfaces;
using FinancePlatform.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace FinancePlatform.API.Controllers;

public class CategoriesController : ApiControllerBase
{
    private readonly ICategoryService _categories;

    public CategoriesController(ICategoryService categories) => _categories = categories;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetAll([FromQuery] CategoryType? type) =>
        Ok(await _categories.GetAllAsync(CurrentUserId, type));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CategoryResponse>> GetById(Guid id) =>
        Ok(await _categories.GetByIdAsync(id, CurrentUserId));

    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> Create([FromBody] CreateCategoryRequest request)
    {
        var result = await _categories.CreateAsync(request, CurrentUserId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CategoryResponse>> Update(Guid id, [FromBody] UpdateCategoryRequest request) =>
        Ok(await _categories.UpdateAsync(id, request, CurrentUserId));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _categories.DeleteAsync(id, CurrentUserId);
        return NoContent();
    }
}
