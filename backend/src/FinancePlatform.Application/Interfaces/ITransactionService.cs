using FinancePlatform.Application.DTOs.Transactions;

namespace FinancePlatform.Application.Interfaces;

public interface ITransactionService
{
    Task<PagedResult<TransactionResponse>> GetPagedAsync(TransactionQueryParams query, Guid userId);
    Task<TransactionResponse> GetByIdAsync(Guid id, Guid userId);
    Task<TransactionResponse> CreateAsync(CreateTransactionRequest request, Guid userId);
    Task<TransactionResponse> UpdateAsync(Guid id, UpdateTransactionRequest request, Guid userId);
    Task DeleteAsync(Guid id, Guid userId);
}
