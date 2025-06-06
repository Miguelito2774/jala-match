using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.EmployeeProfiles.TechnicalProfile.UpdateEmployeeProfile;

public sealed class UpdateEmployeeProfileTechnicalCommandHandler
    : ICommandHandler<UpdateEmployeeProfileTechnicalCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmployeeProfileRepository _employeeProfileRepository;

    public UpdateEmployeeProfileTechnicalCommandHandler(
        IApplicationDbContext context,
        IEmployeeProfileRepository employeeProfileRepository
    )
    {
        _context = context;
        _employeeProfileRepository = employeeProfileRepository;
    }

    public async Task<Result> Handle(
        UpdateEmployeeProfileTechnicalCommand request,
        CancellationToken cancellationToken
    )
    {
        EmployeeProfile? profile = await _context.EmployeeProfiles.FirstOrDefaultAsync(
            p => p.UserId == request.UserId,
            cancellationToken
        );

        if (profile == null)
        {
            return Result.Failure(
                new Error("Profile.NotFound", "Employee profile not found", ErrorType.Failure)
            );
        }

        profile.SfiaLevelGeneral = request.SfiaLevelGeneral;
        profile.Mbti = request.Mbti;

        await _employeeProfileRepository.UpdateAsync(profile);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
