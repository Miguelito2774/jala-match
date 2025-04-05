using Domain.Entities.Profiles;
using Domain.Entities.Teams;
using Domain.Entities.Technologies;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Team> Teams { get; }
    DbSet<ProfileVerification> ProfileVerifications { get; }
    DbSet<EmployeeProfile> EmployeeProfiles { get; }
    DbSet<Technology> Technologies { get; }
    DbSet<EmployeeTechnology> EmployeeTechnologies { get; }
    DbSet<TechnologyCategory> TechnologyCategories { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
