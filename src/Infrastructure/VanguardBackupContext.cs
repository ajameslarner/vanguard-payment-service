using System.Diagnostics.CodeAnalysis;
using Domain.Enums;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

[ExcludeFromCodeCoverage]
public partial class VanguardBackupContext : DbContext
{
    public DbSet<Account> Accounts { get; init; }
    public DbSet<Payment> Payments { get; init; }

    public VanguardBackupContext(DbContextOptions options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseInMemoryDatabase("Name=Database:Name");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Payment>();
        modelBuilder.Entity<Account>()
            .HasData(
                new Account
                {
                    Id = Guid.NewGuid(),
                    AccountNumber = "ACC1_disabled",
                    Balance = 2_000_000.00m,
                    Status = AccountStatus.Disabled
                },
                new Account
                {
                    Id = Guid.NewGuid(),
                    AccountNumber = "ACC2_enabled_bacs_fast",
                    Balance = 2_000.00m,
                    Status = AccountStatus.Live,
                    AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.FasterPayments
                },
                new Account
                {
                    Id = Guid.NewGuid(),
                    AccountNumber = "ACC3_enabled_bacs_chaps",
                    Balance = 2_000.00m,
                    Status = AccountStatus.Live,
                    AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps
                },
                new Account
                {
                    Id = Guid.NewGuid(),
                    AccountNumber = "ACC4_enabled_bacs",
                    Balance = 2_000.00m,
                    Status = AccountStatus.Live,
                    AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs
                },
                new Account
                {
                    Id = Guid.NewGuid(),
                    AccountNumber = "ACC5_enabled_fast",
                    Balance = 2_000.00m,
                    Status = AccountStatus.Live,
                    AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments
                }
            );

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
