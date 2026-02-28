using FinancePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinancePlatform.Infrastructure.Persistence.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Name).HasMaxLength(100).IsRequired();
        builder.Property(a => a.Balance).HasColumnType("decimal(18,2)");
        builder.Property(a => a.Currency).HasMaxLength(3).HasDefaultValue("USD");
        builder.Property(a => a.Color).HasMaxLength(20);
        builder.HasIndex(a => a.UserId);

        builder.HasOne(a => a.User)
            .WithMany(u => u.Accounts)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
