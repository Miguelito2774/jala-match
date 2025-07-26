using System.Threading.Channels;
using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Domain.Entities.Profiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public interface IBackgroundNotificationQueue
{
    void QueueTeamMemberAdded(Guid employeeId, string teamName, string managerEmail);
    void QueueTeamMemberRemoved(Guid employeeId, string teamName, string managerEmail);
    void QueueTeamMemberMoved(
        Guid employeeId,
        string sourceTeamName,
        string targetTeamName,
        string managerEmail
    );
}

public class BackgroundNotificationService : BackgroundService, IBackgroundNotificationQueue
{
    private readonly Channel<NotificationItem> _queue;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackgroundNotificationService> _logger;

    public BackgroundNotificationService(
        IServiceProvider serviceProvider,
        ILogger<BackgroundNotificationService> logger
    )
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        // Create a channel with unlimited capacity
        var options = new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false,
        };
        _queue = Channel.CreateBounded<NotificationItem>(options);
    }

    public void QueueTeamMemberAdded(Guid employeeId, string teamName, string managerEmail)
    {
        var item = new NotificationItem
        {
            Type = NotificationType.TeamMemberAdded,
            EmployeeId = employeeId,
            TeamName = teamName,
            ManagerEmail = managerEmail,
        };

        if (!_queue.Writer.TryWrite(item))
        {
            _logger.LogWarning(
                "Failed to queue team member added notification for employee {EmployeeId}",
                employeeId
            );
        }
    }

    public void QueueTeamMemberRemoved(Guid employeeId, string teamName, string managerEmail)
    {
        var item = new NotificationItem
        {
            Type = NotificationType.TeamMemberRemoved,
            EmployeeId = employeeId,
            TeamName = teamName,
            ManagerEmail = managerEmail,
        };

        if (!_queue.Writer.TryWrite(item))
        {
            _logger.LogWarning(
                "Failed to queue team member removed notification for employee {EmployeeId}",
                employeeId
            );
        }
    }

    public void QueueTeamMemberMoved(
        Guid employeeId,
        string sourceTeamName,
        string targetTeamName,
        string managerEmail
    )
    {
        var item = new NotificationItem
        {
            Type = NotificationType.TeamMemberMoved,
            EmployeeId = employeeId,
            SourceTeamName = sourceTeamName,
            TargetTeamName = targetTeamName,
            ManagerEmail = managerEmail,
        };

        if (!_queue.Writer.TryWrite(item))
        {
            _logger.LogWarning(
                "Failed to queue team member moved notification for employee {EmployeeId}",
                employeeId
            );
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background notification service started");

        await foreach (NotificationItem notification in _queue.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                // Use a separate cancellation token that doesn't depend on the request lifecycle
                using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));
                CancellationToken independentToken = cts.Token;

                await ProcessNotification(notification, independentToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error processing notification for employee {EmployeeId}",
                    notification.EmployeeId
                );

                // Wait a bit before processing next notification to avoid overwhelming the system
                await Task.Delay(1000, stoppingToken);
            }
        }
    }

    private async Task ProcessNotification(
        NotificationItem notification,
        CancellationToken cancellationToken
    )
    {
        const int maxRetries = 3;
        int attempt = 0;

        while (attempt < maxRetries)
        {
            try
            {
                using IServiceScope scope = _serviceProvider.CreateScope();
                IEmployeeProfileRepository employeeRepository =
                    scope.ServiceProvider.GetRequiredService<IEmployeeProfileRepository>();
                INotificationService notificationService =
                    scope.ServiceProvider.GetRequiredService<INotificationService>();

                EmployeeProfile? profile = await employeeRepository.GetByIdWithUserAsync(
                    notification.EmployeeId,
                    cancellationToken
                );

                if (profile?.User?.Email == null)
                {
                    _logger.LogWarning(
                        "Employee {EmployeeId} not found or has no email",
                        notification.EmployeeId
                    );
                    return; // Don't retry if employee doesn't exist
                }

                string memberName = $"{profile.FirstName} {profile.LastName}";

                switch (notification.Type)
                {
                    case NotificationType.TeamMemberAdded:
                        _logger.LogInformation(
                            "Sending team member added notification to {Email} for team {TeamName}",
                            profile.User.Email,
                            notification.TeamName
                        );

                        await notificationService.SendTeamMemberAddedNotificationAsync(
                            profile.User.Email,
                            memberName,
                            notification.TeamName,
                            notification.ManagerEmail,
                            cancellationToken
                        );
                        break;

                    case NotificationType.TeamMemberRemoved:
                        _logger.LogInformation(
                            "Sending team member removed notification to {Email} for team {TeamName}",
                            profile.User.Email,
                            notification.TeamName
                        );

                        await notificationService.SendTeamMemberRemovedNotificationAsync(
                            profile.User.Email,
                            memberName,
                            notification.TeamName,
                            notification.ManagerEmail,
                            cancellationToken
                        );
                        break;

                    case NotificationType.TeamMemberMoved:
                        _logger.LogInformation(
                            "Sending team member moved notification to {Email} from {SourceTeam} to {TargetTeam}",
                            profile.User.Email,
                            notification.SourceTeamName,
                            notification.TargetTeamName
                        );

                        await notificationService.SendTeamMemberMovedNotificationAsync(
                            profile.User.Email,
                            memberName,
                            notification.SourceTeamName ?? "Unknown Team",
                            notification.TargetTeamName ?? "Unknown Team",
                            notification.ManagerEmail,
                            cancellationToken
                        );
                        break;
                }

                _logger.LogInformation(
                    "Successfully sent {NotificationType} notification to {Email}",
                    notification.Type,
                    profile.User.Email
                );
                return; // Success, exit retry loop
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Notification processing was cancelled for employee {EmployeeId}",
                    notification.EmployeeId
                );
                return; // Don't retry on cancellation
            }
            catch (Exception ex) when (attempt < maxRetries - 1)
            {
                attempt++;
                _logger.LogWarning(
                    ex,
                    "Failed to send {NotificationType} notification to employee {EmployeeId}, attempt {Attempt}/{MaxRetries}: {Message}",
                    notification.Type,
                    notification.EmployeeId,
                    attempt,
                    maxRetries,
                    ex.Message
                );

                // Exponential backoff
                var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
                await Task.Delay(delay, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to send {NotificationType} notification to employee {EmployeeId} after {MaxRetries} attempts: {Message}",
                    notification.Type,
                    notification.EmployeeId,
                    maxRetries,
                    ex.Message
                );
                return; // Give up after max retries
            }
        }
    }
}

public class NotificationItem
{
    public NotificationType Type { get; set; }
    public Guid EmployeeId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public string? SourceTeamName { get; set; }
    public string? TargetTeamName { get; set; }
    public string ManagerEmail { get; set; } = string.Empty;
}

public enum NotificationType
{
    TeamMemberAdded,
    TeamMemberRemoved,
    TeamMemberMoved,
}
