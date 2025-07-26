using Application.Abstractions.Repositories;
using Domain.Entities.Profiles;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EmployeeProfileRepository : IEmployeeProfileRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeProfileRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EmployeeProfile?> GetByIdAsync(Guid id)
    {
        return await _context
            .EmployeeProfiles.Include(p => p.User) 
            .Include(p => p.Technologies)
            .Include(p => p.Languages)
            .Include(p => p.WorkExperiences)
            .Include(p => p.TeamMemberships)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<EmployeeProfile?> GetByIdWithUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context
            .EmployeeProfiles
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task AddAsync(EmployeeProfile profile)
    {
        await _context.EmployeeProfiles.AddAsync(profile);
    }

    public Task UpdateAsync(EmployeeProfile profile)
    {
        _context.EmployeeProfiles.Update(profile);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(EmployeeProfile profile)
    {
        _context.EmployeeProfiles.Remove(profile);
        return Task.CompletedTask;
    }

    public async Task<List<EmployeeProfile>> GetAvailableProfilesAsync()
    {
        return await _context
            .EmployeeProfiles.Include(p => p.User) 
            .Where(p => p.Availability)
            .ToListAsync();
    }

    public async Task<EmployeeProfile?> GetByUserIdWithAllDataAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .EmployeeProfiles.Include(p => p.User)
            .Include(p => p.Technologies)
            .ThenInclude(et => et.Technology)
            .ThenInclude(t => t.Category)
            .Include(p => p.Languages)
            .Include(p => p.WorkExperiences)
            .Include(p => p.PersonalInterests)
            .Include(p => p.TeamMemberships)
            .ThenInclude(tm => tm.Team)
            .ThenInclude(t => t!.RequiredTechnologies)
            .ThenInclude(rt => rt.Technology)
            .Include(p => p.SpecializedRoles)
            .ThenInclude(sr => sr.SpecializedRole)
            .ThenInclude(sr => sr.TechnicalArea)
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
    }
}
