using FinancePlatform.Application.DTOs.Dashboard;

namespace FinancePlatform.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryResponse> GetSummaryAsync(Guid userId);
}
