using SharedKernel.Results;

namespace Application.Abstractions.Services;

public interface INotificationService
{
    Task<Result> SendProfileApprovedNotificationAsync(
        string recipientEmail,
        string recipientName,
        int? proposedSfiaLevel,
        string? notes,
        CancellationToken cancellationToken = default
    );

    Task<Result> SendProfileRejectedNotificationAsync(
        string recipientEmail,
        string recipientName,
        string rejectionReason,
        CancellationToken cancellationToken = default
    );

    Task<Result> SendTeamDeletedNotificationAsync(
        List<string> memberEmails,
        List<string> memberNames,
        string teamName,
        string managerEmail,
        CancellationToken cancellationToken = default
    );

    Task<Result> SendTeamMemberAddedNotificationAsync(
        string recipientEmail,
        string recipientName,
        string teamName,
        string managerEmail,
        CancellationToken cancellationToken = default
    );

    Task<Result> SendTeamMemberRemovedNotificationAsync(
        string recipientEmail,
        string recipientName,
        string teamName,
        string managerEmail,
        CancellationToken cancellationToken = default
    );

    Task<Result> SendTeamMemberMovedNotificationAsync(
        string recipientEmail,
        string recipientName,
        string fromTeamName,
        string toTeamName,
        string managerEmail,
        CancellationToken cancellationToken = default
    );

    Task<Result> SendInvitationNotificationAsync(
        string recipientEmail,
        string recipientName,
        string invitationLink,
        string adminEmail,
        CancellationToken cancellationToken = default
    );

    Task<Result> SendPasswordResetNotificationAsync(
        string recipientEmail,
        string recipientName,
        string resetLink,
        CancellationToken cancellationToken = default
    );
}
