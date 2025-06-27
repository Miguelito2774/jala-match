using Domain.Entities.Profiles;

namespace Application.Abstractions.Repositories;

public interface IEmployeeProfileRepository
{
    Task<EmployeeProfile?> GetByIdAsync(Guid id);
    Task<EmployeeProfile?> GetByUserIdWithAllDataAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(EmployeeProfile profile);
    Task UpdateAsync(EmployeeProfile profile);
    Task DeleteAsync(EmployeeProfile profile);
    Task<List<EmployeeProfile>> GetAvailableProfilesAsync();
}
