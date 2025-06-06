using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.DTOs;
using Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Queries.EmployeeProfiles.Languages.GetById;

public sealed class GetEmployeeLanguageByIdQueryHandler
    : IQueryHandler<GetEmployeeLanguageByIdQuery, EmployeeLanguageDto>
{
    private readonly IApplicationDbContext _context;

    public GetEmployeeLanguageByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<EmployeeLanguageDto>> Handle(
        GetEmployeeLanguageByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        EmployeeLanguage? entity = await _context.EmployeeLanguages.FirstOrDefaultAsync(
            el => el.Id == request.LanguageId,
            cancellationToken
        );

        if (entity == null)
        {
            return Result.Failure<EmployeeLanguageDto>(
                new SharedKernel.Errors.Error(
                    "EmployeeLanguage.NotFound",
                    "Language entry not found",
                    SharedKernel.Errors.ErrorType.Failure
                )
            );
        }

        var dto = new EmployeeLanguageDto
        {
            Id = entity.Id,
            Language = entity.Language,
            Proficiency = entity.Proficiency,
        };

        return Result.Success(dto);
    }
}
