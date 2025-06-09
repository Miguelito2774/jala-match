using Application.DTOs;
using Domain.Entities.Enums;
using SharedKernel.Results;

namespace Application.Abstractions.Services;

public interface IAuthService
{
    Task<Result<AuthResponse>> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken
    );
    Task<Result<AuthResponse>> RegisterEmployeeAsync(
        RegisterEmployeeRequest request,
        CancellationToken cancellationToken
    );
    Task<Result<AuthResponse>> RegisterManagerAsync(
        RegisterManagerRequest request,
        string invitationToken,
        CancellationToken cancellationToken
    );
    Task<Result<string>> CreateInvitationLinkAsync(
        Guid adminId,
        string email,
        Role targetRole,
        CancellationToken cancellationToken
    );
    Task<Result<bool>> ValidateInvitationTokenAsync(
        string token,
        CancellationToken cancellationToken
    );
    Task<Result<bool>> RequestPasswordResetAsync(
        string email,
        CancellationToken cancellationToken
    );
    Task<Result<bool>> ResetPasswordAsync(
        string token,
        string newPassword,
        CancellationToken cancellationToken
    );
}
