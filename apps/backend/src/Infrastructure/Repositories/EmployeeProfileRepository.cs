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
            .EmployeeProfiles
            .Include(p => p.User) // Include User to load email information
            .Include(p => p.Technologies)
            .Include(p => p.Languages)
            .Include(p => p.WorkExperiences)
            .Include(p => p.TeamMemberships)
            .FirstOrDefaultAsync(p => p.Id == id);
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
            .EmployeeProfiles
            .Include(p => p.User) // Include User to load email information
            .Where(p => p.Availability)
            .ToListAsync();
    }
}
