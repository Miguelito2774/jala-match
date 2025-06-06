using Application.Abstractions.Data;
using Domain.Entities.Areas_Roles;
using Domain.Entities.Invitations;
using Domain.Entities.Profiles;
using Domain.Entities.Teams;
using Domain.Entities.Technologies;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<EmployeeProfile> EmployeeProfiles => Set<EmployeeProfile>();
    public DbSet<TechnologyCategory> TechnologyCategories => Set<TechnologyCategory>();
    public DbSet<Technology> Technologies => Set<Technology>();
    public DbSet<EmployeeTechnology> EmployeeTechnologies => Set<EmployeeTechnology>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<ProfileVerification> ProfileVerifications => Set<ProfileVerification>();
    public DbSet<WorkExperience> WorkExperiences => Set<WorkExperience>();
    public DbSet<PersonalInterest> PersonalInterests => Set<PersonalInterest>();
    public DbSet<EmployeeLanguage> EmployeeLanguages => Set<EmployeeLanguage>();
    public DbSet<TeamRequiredTechnology> TeamRequiredTechnologies => Set<TeamRequiredTechnology>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
    public DbSet<EmployeeSpecializedRole> EmployeeSpecializedRoles =>
        Set<EmployeeSpecializedRole>();
    public DbSet<SpecializedRole> SpecializedRoles => Set<SpecializedRole>();

    public DbSet<SpecializedRoleSkill> SpecializedRoleSkills => Set<SpecializedRoleSkill>();
    public DbSet<TechnicalArea> TechnicalAreas => Set<TechnicalArea>();
    public DbSet<RecommendedMember> RecommendedMembers => Set<RecommendedMember>();
    public DbSet<InvitationLink> InvitationLinks => Set<InvitationLink>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        modelBuilder.HasDefaultSchema(Schemas.Default);
    }
}
