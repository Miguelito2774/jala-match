namespace Application.Abstractions.Services;

public interface ISfiaCalculatorService
{
    Task<int> CalculateAverageSfiaForRequirements(
        Guid employeeProfileId,
        List<string> requiredTechnologies,
        CancellationToken cancellationToken
    );
}
