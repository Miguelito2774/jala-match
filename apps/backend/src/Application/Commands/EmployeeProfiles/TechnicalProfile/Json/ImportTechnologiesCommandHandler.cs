using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.DTOs;
using Domain.Entities.Profiles;
using Domain.Entities.Technologies;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.EmployeeProfiles.TechnicalProfile.Json;

internal sealed class ImportTechnologiesCommandHandler(IApplicationDbContext context)
    : ICommandHandler<ImportTechnologiesCommand, List<Guid>>
{
    public async Task<Result<List<Guid>>> Handle(
        ImportTechnologiesCommand request,
        CancellationToken cancellationToken
    )
    {
        EmployeeProfile? employeeProfile = await context.EmployeeProfiles.FirstOrDefaultAsync(
            ep => ep.UserId == request.UserId,
            cancellationToken
        );

        if (employeeProfile is null)
        {
            return Result.Failure<List<Guid>>(
                Error.NotFound("Profile.NotFound", "Employee profile not found")
            );
        }

        var employeeTechnologies = new List<EmployeeTechnology>();
        var createdIds = new List<Guid>();

        // Get all technologies and categories in memory to avoid EF translation issues
        List<Technology> allTechnologies = await context
            .Technologies.Include(t => t.Category)
            .ToListAsync(cancellationToken);

        // Get existing employee technologies to avoid duplicates
        List<Guid> existingTechnologies = await context
            .EmployeeTechnologies.Where(et => et.EmployeeProfileId == employeeProfile.Id)
            .Select(et => et.TechnologyId)
            .ToListAsync(cancellationToken);

        foreach (TechnologyImportDto dto in request.Technologies)
        {
            // Find technology by name and category (case-insensitive comparison in memory)
            Technology? technology = allTechnologies.FirstOrDefault(t =>
                string.Equals(t.Name, dto.Name, StringComparison.OrdinalIgnoreCase)
                && string.Equals(t.Category.Name, dto.Category, StringComparison.OrdinalIgnoreCase)
            );

            if (technology is null)
            {
                // Skip this technology if not found
                continue;
            }

            // Check if employee already has this technology
            if (existingTechnologies.Contains(technology.Id))
            {
                // Skip if technology already exists for this employee
                continue;
            }

            var employeeTechnology = new EmployeeTechnology
            {
                Id = Guid.NewGuid(),
                EmployeeProfileId = employeeProfile.Id,
                TechnologyId = technology.Id,
                SfiaLevel = dto.SfiaLevel,
                YearsExperience = dto.YearsExperience,
                Version = dto.Version,
                EmployeeProfile = employeeProfile,
                Technology = technology,
            };

            employeeTechnologies.Add(employeeTechnology);
            createdIds.Add(employeeTechnology.Id);
        }

        context.EmployeeTechnologies.AddRange(employeeTechnologies);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(createdIds);
    }
}
