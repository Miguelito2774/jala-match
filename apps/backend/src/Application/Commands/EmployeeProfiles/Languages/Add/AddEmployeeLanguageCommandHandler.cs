using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.EmployeeProfiles.Languages.Add;

public sealed class AddEmployeeLanguageCommandHandler
    : ICommandHandler<AddEmployeeLanguageCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public AddEmployeeLanguageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(
        AddEmployeeLanguageCommand request,
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

        bool existingLanguage = await _context.EmployeeLanguages.AnyAsync(
            el => el.EmployeeProfileId == profile.Id && el.Language == request.Language,
            cancellationToken
        );

        if (existingLanguage)
        {
            return Result.Failure<Guid>(
                new Error(
                    "Language.AlreadyExists",
                    "Language already exists for this employee",
                    ErrorType.Failure
                )
            );
        }

        var employeeLanguage = new EmployeeLanguage
        {
            EmployeeProfileId = profile.Id,
            Language = request.Language,
            Proficiency = request.Proficiency,
        };

        _context.EmployeeLanguages.Add(employeeLanguage);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(employeeLanguage.Id);
    }
}
