using FinancePlatform.Application.DTOs.Dashboard;
using FinancePlatform.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinancePlatform.API.Controllers;

public class DashboardController : ApiControllerBase
{
    private readonly IDashboardService _dashboard;

    public DashboardController(IDashboardService dashboard) => _dashboard = dashboard;

    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryResponse>> GetSummary() =>
        Ok(await _dashboard.GetSummaryAsync(CurrentUserId));
}
