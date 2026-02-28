using FinancePlatform.Application.DTOs.Auth;

namespace FinancePlatform.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<UserProfileResponse> GetProfileAsync(Guid userId);
}
