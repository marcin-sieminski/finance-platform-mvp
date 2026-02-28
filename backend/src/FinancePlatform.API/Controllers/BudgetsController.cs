using FinancePlatform.Application.DTOs.Budgets;
using FinancePlatform.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinancePlatform.API.Controllers;

public class BudgetsController : ApiControllerBase
{
    private readonly IBudgetService _budgets;

    public BudgetsController(IBudgetService budgets) => _budgets = budgets;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BudgetResponse>>> GetByPeriod(
        [FromQuery] int? month, [FromQuery] int? year)
    {
        var now = DateTime.UtcNow;
        return Ok(await _budgets.GetByPeriodAsync(CurrentUserId, month ?? now.Month, year ?? now.Year));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BudgetResponse>> GetById(Guid id) =>
        Ok(await _budgets.GetByIdAsync(id, CurrentUserId));

    [HttpPost]
    public async Task<ActionResult<BudgetResponse>> Create([FromBody] CreateBudgetRequest request)
    {
        var result = await _budgets.CreateAsync(request, CurrentUserId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BudgetResponse>> Update(Guid id, [FromBody] UpdateBudgetRequest request) =>
        Ok(await _budgets.UpdateAsync(id, request, CurrentUserId));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _budgets.DeleteAsync(id, CurrentUserId);
        return NoContent();
    }
}
