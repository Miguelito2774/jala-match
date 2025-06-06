using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.EmployeeProfiles.Languages.DeleteEmployeeLanguage;

public sealed class DeleteEmployeeLanguageCommandHandler
    : ICommandHandler<DeleteEmployeeLanguageCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteEmployeeLanguageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(
        DeleteEmployeeLanguageCommand request,
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

        _context.EmployeeLanguages.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
