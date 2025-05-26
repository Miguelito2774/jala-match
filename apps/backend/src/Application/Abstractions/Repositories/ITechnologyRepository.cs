using Domain.Entities.Teams;
using Domain.Entities.Technologies;

namespace Application.Abstractions.Repositories;

public interface ITechnologyRepository
{
    Task<Technology?> GetByNameAsync(string name);
    Task AddAsync(Technology technology);
    Task<bool> ExistsByNameAsync(string name);
}
