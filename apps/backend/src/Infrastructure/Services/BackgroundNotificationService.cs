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
    void QueueTeamMemberAdded(string recipientEmail, string recipientName, string teamName, string managerEmail);
    void QueueTeamMemberRemoved(string recipientEmail, string recipientName, string teamName, string managerEmail);
    void QueueTeamMemberMoved(
        string recipientEmail,
        string recipientName,
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

    public void QueueTeamMemberAdded(string recipientEmail, string recipientName, string teamName, string managerEmail)
    {
        var item = new NotificationItem
        {
            Type = NotificationType.TeamMemberAdded,
            RecipientEmail = recipientEmail,
            RecipientName = recipientName,
            TeamName = teamName,
            ManagerEmail = managerEmail,
        };

        if (!_queue.Writer.TryWrite(item))
        {
            _logger.LogWarning(
                "Failed to queue team member added notification for {Email}",
                recipientEmail
            );
        }
    }

    public void QueueTeamMemberRemoved(string recipientEmail, string recipientName, string teamName, string managerEmail)
    {
        var item = new NotificationItem
        {
            Type = NotificationType.TeamMemberRemoved,
            RecipientEmail = recipientEmail,
            RecipientName = recipientName,
            TeamName = teamName,
            ManagerEmail = managerEmail,
        };

        if (!_queue.Writer.TryWrite(item))
        {
            _logger.LogWarning(
                "Failed to queue team member removed notification for {Email}",
                recipientEmail
            );
        }
    }

    public void QueueTeamMemberMoved(
        string recipientEmail,
        string recipientName,
        string sourceTeamName,
        string targetTeamName,
        string managerEmail
    )
    {
        var item = new NotificationItem
        {
            Type = NotificationType.TeamMemberMoved,
            RecipientEmail = recipientEmail,
            RecipientName = recipientName,
            TeamName = sourceTeamName,
            TargetTeamName = targetTeamName,
            ManagerEmail = managerEmail,
        };

        if (!_queue.Writer.TryWrite(item))
        {
            _logger.LogWarning(
                "Failed to queue team member moved notification for {Email}",
                recipientEmail
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
                    "Error processing notification for {Email}",
                    notification.RecipientEmail
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
                INotificationService notificationService =
                    scope.ServiceProvider.GetRequiredService<INotificationService>();

                switch (notification.Type)
                {
                    case NotificationType.TeamMemberAdded:
                        _logger.LogInformation(
                            "Sending team member added notification to {Email} for team {TeamName}",
                            notification.RecipientEmail,
                            notification.TeamName
                        );

                        await notificationService.SendTeamMemberAddedNotificationAsync(
                            notification.RecipientEmail,
                            notification.RecipientName,
                            notification.TeamName,
                            notification.ManagerEmail,
                            cancellationToken
                        );
                        break;

                    case NotificationType.TeamMemberRemoved:
                        _logger.LogInformation(
                            "Sending team member removed notification to {Email} for team {TeamName}",
                            notification.RecipientEmail,
                            notification.TeamName
                        );

                        await notificationService.SendTeamMemberRemovedNotificationAsync(
                            notification.RecipientEmail,
                            notification.RecipientName,
                            notification.TeamName,
                            notification.ManagerEmail,
                            cancellationToken
                        );
                        break;

                    case NotificationType.TeamMemberMoved:
                        _logger.LogInformation(
                            "Sending team member moved notification to {Email} from {SourceTeam} to {TargetTeam}",
                            notification.RecipientEmail,
                            notification.TeamName,
                            notification.TargetTeamName
                        );

                        await notificationService.SendTeamMemberMovedNotificationAsync(
                            notification.RecipientEmail,
                            notification.RecipientName,
                            notification.TeamName,
                            notification.TargetTeamName!,
                            notification.ManagerEmail,
                            cancellationToken
                        );
                        break;
                }

                _logger.LogInformation(
                    "Successfully sent {NotificationType} notification to {Email}",
                    notification.Type,
                    notification.RecipientEmail
                );
                return; // Success, exit retry loop
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogInformation(
                    ex,
                    "Notification processing was cancelled for {Email}",
                    notification.RecipientEmail
                );
                return; // Don't retry on cancellation
            }
            catch (Exception ex) when (attempt < maxRetries - 1)
            {
                attempt++;
                _logger.LogWarning(
                    ex,
                    "Failed to send {NotificationType} notification to {Email}, attempt {Attempt}/{MaxRetries}: {Message}",
                    notification.Type,
                    notification.RecipientEmail,
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
                    "Failed to send {NotificationType} notification to {Email} after {MaxRetries} attempts: {Message}",
                    notification.Type,
                    notification.RecipientEmail,
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
    public string RecipientEmail { get; set; } = string.Empty;
    public string RecipientName { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public string? TargetTeamName { get; set; }
    public string ManagerEmail { get; set; } = string.Empty;
}

public enum NotificationType
{
    TeamMemberAdded,
    TeamMemberRemoved,
    TeamMemberMoved,
}
