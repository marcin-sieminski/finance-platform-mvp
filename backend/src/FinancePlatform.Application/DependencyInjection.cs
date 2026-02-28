using FinancePlatform.Application.Interfaces;
using FinancePlatform.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FinancePlatform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IBudgetService, BudgetService>();
        services.AddScoped<IDashboardService, DashboardService>();
        return services;
    }
}
