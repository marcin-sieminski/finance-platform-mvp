using FinancePlatform.Application.DTOs.Transactions;
using FinancePlatform.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinancePlatform.API.Controllers;

public class TransactionsController : ApiControllerBase
{
    private readonly ITransactionService _transactions;

    public TransactionsController(ITransactionService transactions) => _transactions = transactions;

    [HttpGet]
    public async Task<ActionResult<PagedResult<TransactionResponse>>> GetPaged([FromQuery] TransactionQueryParams query) =>
        Ok(await _transactions.GetPagedAsync(query, CurrentUserId));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TransactionResponse>> GetById(Guid id) =>
        Ok(await _transactions.GetByIdAsync(id, CurrentUserId));

    [HttpPost]
    public async Task<ActionResult<TransactionResponse>> Create([FromBody] CreateTransactionRequest request)
    {
        var result = await _transactions.CreateAsync(request, CurrentUserId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TransactionResponse>> Update(Guid id, [FromBody] UpdateTransactionRequest request) =>
        Ok(await _transactions.UpdateAsync(id, request, CurrentUserId));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _transactions.DeleteAsync(id, CurrentUserId);
        return NoContent();
    }
}
