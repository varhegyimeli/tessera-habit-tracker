namespace Tessera.Data;

using Microsoft.EntityFrameworkCore;
using Tessera.Core.Models;

/// <summary>
/// Entity Framework Core DbContext for the Tessera habit tracker application.
/// </summary>
public class TesseraDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the TesseraDbContext.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    public TesseraDbContext(DbContextOptions<TesseraDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the Habits DbSet.
    /// </summary>
    public DbSet<Habit> Habits { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Completions DbSet.
    /// </summary>
    public DbSet<HabitCompletion> Completions { get; set; } = null!;

    /// <summary>
    /// Configures the model using the Fluent API.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Habit entity
        modelBuilder.Entity<Habit>(entity =>
        {
            entity.HasKey(h => h.Id);

            entity.Property(h => h.Name)
                .IsRequired();

            entity.Property(h => h.Color)
                .IsRequired();

            entity.Property(h => h.Description)
                .IsRequired(false);

            entity.Property(h => h.CreatedAt)
                .IsRequired();
        });

        // Configure HabitCompletion entity
        modelBuilder.Entity<HabitCompletion>(entity =>
        {
            entity.HasKey(hc => hc.Id);

            entity.Property(hc => hc.HabitId)
                .IsRequired();

            entity.Property(hc => hc.Date)
                .IsRequired()
                .HasConversion(
                    d => d.ToString("O"),
                    s => DateOnly.ParseExact(s, "O"));

            // Unique constraint on (HabitId, Date) to prevent duplicate completions
            entity.HasIndex(hc => new { hc.HabitId, hc.Date })
                .IsUnique();

            // Foreign key relationship to Habit
            entity.HasOne<Habit>()
                .WithMany()
                .HasForeignKey(hc => hc.HabitId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}