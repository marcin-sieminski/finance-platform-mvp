using FinancePlatform.Application.DTOs.Auth;
using FinancePlatform.Application.Interfaces;
using FinancePlatform.Domain.Entities;
using FinancePlatform.Domain.Enums;
using FinancePlatform.Domain.Interfaces;

namespace FinancePlatform.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly ICategoryRepository _categories;
    private readonly IJwtService _jwt;
    private readonly IPasswordHasher _hasher;

    public AuthService(IUserRepository users, ICategoryRepository categories, IJwtService jwt, IPasswordHasher hasher)
    {
        _users = users;
        _categories = categories;
        _jwt = jwt;
        _hasher = hasher;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _users.ExistsAsync(request.Email))
            throw new InvalidOperationException("Email already registered.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email.ToLowerInvariant(),
            PasswordHash = _hasher.Hash(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _users.CreateAsync(user);
        await SeedDefaultCategoriesAsync(user.Id);

        return new AuthResponse
        {
            Token = _jwt.GenerateToken(user),
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ExpiresAt = _jwt.GetExpiry()
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _users.GetByEmailAsync(request.Email.ToLowerInvariant())
            ?? throw new UnauthorizedAccessException("Invalid email or password.");

        if (!_hasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return new AuthResponse
        {
            Token = _jwt.GenerateToken(user),
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ExpiresAt = _jwt.GetExpiry()
        };
    }

    public async Task<UserProfileResponse> GetProfileAsync(Guid userId)
    {
        var user = await _users.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");

        return new UserProfileResponse
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt
        };
    }

    private async Task SeedDefaultCategoriesAsync(Guid userId)
    {
        var now = DateTime.UtcNow;
        var defaults = new[]
        {
            ("Salary", CategoryType.Income, "💼", "#22c55e"),
            ("Freelance", CategoryType.Income, "💻", "#16a34a"),
            ("Investment", CategoryType.Income, "📈", "#15803d"),
            ("Other Income", CategoryType.Income, "💰", "#4ade80"),
            ("Groceries", CategoryType.Expense, "🛒", "#ef4444"),
            ("Transport", CategoryType.Expense, "🚗", "#f97316"),
            ("Housing", CategoryType.Expense, "🏠", "#eab308"),
            ("Utilities", CategoryType.Expense, "💡", "#84cc16"),
            ("Healthcare", CategoryType.Expense, "🏥", "#06b6d4"),
            ("Entertainment", CategoryType.Expense, "🎬", "#8b5cf6"),
            ("Dining Out", CategoryType.Expense, "🍽️", "#ec4899"),
            ("Shopping", CategoryType.Expense, "🛍️", "#f43f5e"),
            ("Education", CategoryType.Expense, "📚", "#3b82f6"),
            ("Other", CategoryType.Expense, "📦", "#6b7280"),
        };

        var categories = defaults.Select(d => new Category
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = d.Item1,
            Type = d.Item2,
            Icon = d.Item3,
            Color = d.Item4,
            IsDefault = true,
            CreatedAt = now,
            UpdatedAt = now
        });

        await _categories.CreateRangeAsync(categories);
    }
}
