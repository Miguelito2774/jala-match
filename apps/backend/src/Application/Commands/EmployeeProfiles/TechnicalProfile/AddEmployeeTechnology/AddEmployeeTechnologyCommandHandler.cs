using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Entities.Profiles;
using Domain.Entities.Technologies;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.EmployeeProfiles.TechnicalProfile.AddEmployeeTechnology;

public sealed class AddEmployeeTechnologyCommandHandler
    : ICommandHandler<AddEmployeeTechnologyCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public AddEmployeeTechnologyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(
        AddEmployeeTechnologyCommand request,
        CancellationToken cancellationToken
    )
    {
        EmployeeProfile? profile = await _context.EmployeeProfiles.FirstOrDefaultAsync(
            p => p.UserId == request.UserId,
            cancellationToken
        );

        if (profile == null)
        {
            return Result.Failure<Guid>(
                new Error("Profile.NotFound", "Employee profile not found", ErrorType.Failure)
            );
        }

        Technology? technology = await _context.Technologies.FirstOrDefaultAsync(
            t => t.Id == request.TechnologyId,
            cancellationToken
        );

        if (technology == null)
        {
            return Result.Failure<Guid>(
                new Error("Technology.NotFound", "Technology not found", ErrorType.Failure)
            );
        }

        bool existingTechnology = await _context.EmployeeTechnologies.AnyAsync(
            et => et.EmployeeProfileId == profile.Id && et.TechnologyId == request.TechnologyId,
            cancellationToken
        );

        if (existingTechnology)
        {
            return Result.Failure<Guid>(
                new Error(
                    "Technology.AlreadyExists",
                    "Technology already exists for this employee",
                    ErrorType.Failure
                )
            );
        }

        var employeeTechnology = new EmployeeTechnology
        {
            EmployeeProfileId = profile.Id,
            TechnologyId = request.TechnologyId,
            YearsExperience = request.YearsExperience,
            SfiaLevel = request.SfiaLevel,
            Version = request.Version,
        };

        _context.EmployeeTechnologies.Add(employeeTechnology);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(employeeTechnology.Id);
    }
}
