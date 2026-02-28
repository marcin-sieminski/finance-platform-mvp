using FinancePlatform.Application.DTOs.Accounts;
using FinancePlatform.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinancePlatform.API.Controllers;

public class AccountsController : ApiControllerBase
{
    private readonly IAccountService _accounts;

    public AccountsController(IAccountService accounts) => _accounts = accounts;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccountResponse>>> GetAll() =>
        Ok(await _accounts.GetAllAsync(CurrentUserId));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AccountResponse>> GetById(Guid id) =>
        Ok(await _accounts.GetByIdAsync(id, CurrentUserId));

    [HttpPost]
    public async Task<ActionResult<AccountResponse>> Create([FromBody] CreateAccountRequest request)
    {
        var result = await _accounts.CreateAsync(request, CurrentUserId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AccountResponse>> Update(Guid id, [FromBody] UpdateAccountRequest request) =>
        Ok(await _accounts.UpdateAsync(id, request, CurrentUserId));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _accounts.DeleteAsync(id, CurrentUserId);
        return NoContent();
    }
}
