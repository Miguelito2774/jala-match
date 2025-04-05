using Application.Abstractions.Data;
using Domain.Entities.Profiles;
using Domain.Entities.Teams;
using Domain.Entities.Technologies;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options
) : DbContext(options), IApplicationDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<EmployeeProfile> EmployeeProfiles => Set<EmployeeProfile>();
    public DbSet<TechnologyCategory> TechnologyCategories => Set<TechnologyCategory>();
    public DbSet<Technology> Technologies => Set<Technology>();
    public DbSet<EmployeeTechnology> EmployeeTechnologies => Set<EmployeeTechnology>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<ProfileVerification> ProfileVerifications => Set<ProfileVerification>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.HasDefaultSchema(Schemas.Default);
    }
}
