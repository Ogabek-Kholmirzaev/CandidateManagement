using CandidateManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CandidateManagement.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Candidate> Candidates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Candidate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(256);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.LinkedInProfileUrl).HasMaxLength(256);
            entity.Property(e => e.GithubProfileUrl).HasMaxLength(256);
            entity.Property(e => e.Comment).IsRequired().HasMaxLength(256);

            var startTimeConverter = new ValueConverter<TimeOnly?, TimeSpan?>(
                v => v.HasValue ? v.Value.ToTimeSpan() : null,
                v => v.HasValue ? TimeOnly.FromTimeSpan(v.Value) : null);

            var endTimeConverter = new ValueConverter<TimeOnly?, TimeSpan?>(
                v => v.HasValue ? v.Value.ToTimeSpan() : null,
                v => v.HasValue ? TimeOnly.FromTimeSpan(v.Value) : null);

            entity.Property(e => e.StartCallTime).HasConversion(startTimeConverter);
            entity.Property(e => e.EndCallTime).HasConversion(endTimeConverter);
        });
    }
}
