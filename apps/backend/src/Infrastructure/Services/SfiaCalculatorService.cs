using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Domain.Entities.Technologies;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class SfiaCalculatorService : ISfiaCalculatorService
{
    private readonly IApplicationDbContext _dbContext;

    public SfiaCalculatorService(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> CalculateAverageSfiaForRequirements(
        Guid employeeProfileId,
        List<string> requiredTechnologies,
        CancellationToken cancellationToken
    )
    {
        List<EmployeeTechnology> matchingTechnologies = await _dbContext
            .EmployeeTechnologies.Include(et => et.Technology)
            .Where(et =>
                et.EmployeeProfileId == employeeProfileId
                && requiredTechnologies.Contains(et.Technology.Name)
            )
            .ToListAsync(cancellationToken);

        if (!matchingTechnologies.Any())
        {
            return 0;
        }

        return (int)matchingTechnologies.Average(t => t.SfiaLevel);
    }
}
