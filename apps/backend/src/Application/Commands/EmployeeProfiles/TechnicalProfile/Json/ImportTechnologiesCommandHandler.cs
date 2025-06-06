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

        foreach (TechnologyImportDto dto in request.Technologies)
        {
            // Find technology by name and category
            Technology? technology = await context
                .Technologies.Include(t => t.Category)
                .FirstOrDefaultAsync(
                    t =>
                        t.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase)
                        && t.Category.Name.Equals(dto.Category, StringComparison.OrdinalIgnoreCase),
                    cancellationToken
                );

            if (technology is null)
            {
                // Skip this technology if not found
                continue;
            }

            // Check if employee already has this technology
            bool existingTechnology = await context.EmployeeTechnologies.AnyAsync(
                et =>
                    et.EmployeeProfileId == employeeProfile.Id && et.TechnologyId == technology.Id,
                cancellationToken
            );

            if (existingTechnology)
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
