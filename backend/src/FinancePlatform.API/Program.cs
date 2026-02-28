using System.Text;
using Azure.Identity;
using FinancePlatform.API;
using FinancePlatform.API.Middleware;
using FinancePlatform.Application;
using FinancePlatform.Infrastructure;
using FinancePlatform.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var keyVaultUri = builder.Configuration["KeyVaultUri"];
if (!string.IsNullOrEmpty(keyVaultUri))
{
    builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential());
}

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
            NameClaimType = "sub"
        };
    });

builder.Services.AddCors(opts => opts.AddPolicy("FrontendPolicy", policy =>
    policy.WithOrigins(
        builder.Configuration["AllowedOrigins"]!
            .Split(',', StringSplitOptions.RemoveEmptyEntries))
    .AllowAnyMethod()
    .AllowAnyHeader()));

builder.Services.AddControllers();
builder.Services.AddOpenApi(opts =>
{
    opts.AddDocumentTransformer((document, _, _) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "Finance Platform API",
            Version = "v1"
        };
        return Task.CompletedTask;
    });

    opts.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapOpenApi();
app.MapScalarApiReference();
app.UseCors("FrontendPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
