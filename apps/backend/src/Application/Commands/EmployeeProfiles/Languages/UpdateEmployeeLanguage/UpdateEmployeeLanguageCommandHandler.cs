using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.EmployeeProfiles.Languages.UpdateEmployeeLanguage;

public sealed class UpdateEmployeeLanguageCommandHandler
    : ICommandHandler<UpdateEmployeeLanguageCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateEmployeeLanguageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(
        UpdateEmployeeLanguageCommand request,
        CancellationToken cancellationToken
    )
    {
        EmployeeLanguage? entity = await _context.EmployeeLanguages.FirstOrDefaultAsync(
            el => el.Id == request.LanguageId,
            cancellationToken
        );

        if (entity == null)
        {
            return Result.Failure(
                new Error(
                    "EmployeeLanguage.NotFound",
                    "Language entry not found",
                    ErrorType.Failure
                )
            );
        }

        entity.Language = request.Language;
        entity.Proficiency = request.Proficiency;

        _context.EmployeeLanguages.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
