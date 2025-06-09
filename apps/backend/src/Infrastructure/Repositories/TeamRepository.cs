using Application.Abstractions.Repositories;
using Domain.Entities.Teams;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly ApplicationDbContext _context;

    public TeamRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Team team, CancellationToken ct)
    {
        await _context.Teams.AddAsync(team, ct);
    }

    public Task UpdateAsync(Team team, CancellationToken ct)
    {
        _context.Teams.Update(team);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _context.SaveChangesAsync(ct);
    }

    public async Task<Team?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context
            .Teams
            .Include(t => t.Members)
            .Include(t => t.Creator)
                .ThenInclude(c => c.EmployeeProfile) // Include creator's employee profile for name
            .Include(t => t.RequiredTechnologies)
                .ThenInclude(rt => rt.Technology)
            .FirstOrDefaultAsync(t => t.Id == id, ct);
    }

    public Task DeleteAsync(Team team, CancellationToken ct)
    {
        _context.Teams.Remove(team);
        return Task.CompletedTask;
    }

    public async Task<List<Team>> GetAllAsync(CancellationToken ct)
    {
        return await _context
            .Teams
            .Include(t => t.Members)
            .Include(t => t.Creator)
                .ThenInclude(c => c.EmployeeProfile) // Include creator's employee profile for name
            .Include(t => t.RequiredTechnologies)
                .ThenInclude(rt => rt.Technology)
            .ToListAsync(ct);
    }

    public async Task<List<Team>> GetByCreatorIdAsync(Guid creatorId, CancellationToken ct)
    {
        return await _context
            .Teams
            .Where(t => t.CreatorId == creatorId)
            .Include(t => t.Members)
            .Include(t => t.Creator)
                .ThenInclude(c => c.EmployeeProfile) // Include creator's employee profile for name
            .Include(t => t.RequiredTechnologies)
                .ThenInclude(rt => rt.Technology)
            .ToListAsync(ct);
    }
}
