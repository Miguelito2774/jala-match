using Application.Abstractions.Repositories;
using Domain.Entities.Teams;
using Domain.Entities.Technologies;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TechnologyRepository : ITechnologyRepository
{
    private readonly ApplicationDbContext _context;

    public TechnologyRepository(ApplicationDbContext context) => _context = context;

    public async Task<Technology?> GetByNameAsync(string name) =>
        await _context.Technologies.FirstOrDefaultAsync(t => t.Name == name);

    public async Task AddAsync(Technology technology) =>
        await _context.Technologies.AddAsync(technology);

    public async Task<bool> ExistsByNameAsync(string name) =>
        await _context.Technologies.AnyAsync(t => t.Name == name);

    public async Task<Team?> GetByIdWithDetailsAsync(Guid id) =>
        await _context
            .Teams.Include(t => t.Members)
            .Include(t => t.RequiredTechnologies)
            .ThenInclude(rt => rt.Technology)
            .FirstOrDefaultAsync(t => t.Id == id);

    public async Task<List<Team>> GetAllWithDetailsAsync() =>
        await _context
            .Teams.Include(t => t.Members)
            .Include(t => t.RequiredTechnologies)
            .ThenInclude(rt => rt.Technology)
            .ToListAsync();
}
