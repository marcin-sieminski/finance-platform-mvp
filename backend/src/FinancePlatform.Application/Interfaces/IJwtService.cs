using FinancePlatform.Domain.Entities;

namespace FinancePlatform.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
    DateTime GetExpiry();
}
