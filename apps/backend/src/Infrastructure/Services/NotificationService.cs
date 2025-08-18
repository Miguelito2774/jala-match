using System.Net;
using System.Net.Mail;
using System.Reflection;
using Application.Abstractions.Services;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentEmail.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Infrastructure.Services;

public sealed class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IEmailTemplateService _emailTemplateService;

    // Email configuration
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;
    private readonly bool _enableSsl;
    private readonly int _smtpTimeout;
    private readonly string _senderEmail;
    private readonly string _senderName;

    // Email sending optimization settings - configurable via appsettings
    private readonly int _emailSendDelayMs;
    private readonly int _maxRetryAttempts;
    private readonly int _retryDelayMs;

    public NotificationService(
        ILogger<NotificationService> logger,
        IConfiguration configuration,
        IEmailTemplateService emailTemplateService
    )
    {
        _logger = logger;
        _configuration = configuration;
        _emailTemplateService = emailTemplateService;

        // Load email settings from configuration with fallback defaults
        _emailSendDelayMs = _configuration.GetValue("Email:SendDelayMs", 500);
        _maxRetryAttempts = _configuration.GetValue("Email:MaxRetryAttempts", 3);
        _retryDelayMs = _configuration.GetValue("Email:RetryDelayMs", 1000);

        // Load SMTP configuration with safe parsing
        _smtpHost =
            _configuration["Email:SmtpHost"]
            ?? _configuration["Email:Smtp:Host"]
            ?? "smtp.gmail.com";

        // Safe port parsing with fallback
        int defaultPort = 587;
        string portValue = _configuration["Email:SmtpPort"] ?? _configuration["Email:Smtp:Port"];
        _smtpPort =
            !string.IsNullOrEmpty(portValue) && int.TryParse(portValue, out int parsedPort)
                ? parsedPort
                : defaultPort;

        _smtpUsername =
            _configuration["Email:SmtpUsername"] ?? _configuration["Email:Smtp:Username"] ?? "";
        _smtpPassword =
            _configuration["Email:SmtpPassword"] ?? _configuration["Email:Smtp:Password"] ?? "";

        // Safe SSL parsing
        string sslValue =
            _configuration["Email:EnableSsl"] ?? _configuration["Email:Smtp:EnableSsl"];
        if (!string.IsNullOrEmpty(sslValue) && bool.TryParse(sslValue, out bool parsedSsl))
        {
            _enableSsl = parsedSsl;
        }
        else
        {
            _enableSsl = true; // Default to true for security
        }

        _smtpTimeout = _configuration.GetValue("Email:SmtpTimeoutMs", 30000);
        _senderEmail = _configuration["Email:SenderEmail"] ?? "noreply@jalamatch.com";
        _senderName = _configuration["Email:SenderName"] ?? "Jala Match";

        // Log configuration for debugging
        _logger.LogInformation(
            "Email service configured - Sender: {SenderEmail}, SMTP Host: {SmtpHost}, Port: {SmtpPort}",
            _senderEmail,
            _smtpHost,
            _smtpPort
        );
    }

    public async Task<Result> SendPasswordResetNotificationAsync(
        string recipientEmail,
        string recipientName,
        string resetLink,
        CancellationToken cancellationToken = default
    )
    {
        string subject = "Restablecer contraseña - Jala Match";
        var model = new PasswordResetEmailModel
        {
            RecipientName = recipientName,
            ResetLink = resetLink,
            CompanyName = GetCompanyName(),
        };

        string template = _emailTemplateService.GetPasswordResetTemplate();

        return await SendEmailAsync(
            recipientEmail,
            recipientName,
            subject,
            template,
            model,
            cancellationToken
        );
    }

    public async Task<Result> SendProfileApprovedNotificationAsync(
        string recipientEmail,
        string recipientName,
        int? proposedSfiaLevel,
        string? notes,
        CancellationToken cancellationToken = default
    )
    {
        string subject = "¡Tu perfil ha sido aprobado! - Jala Match";

        // Build conditional sections
        string sfiaLevelSection = proposedSfiaLevel.HasValue
            ? $"<p><strong>Nivel SFIA asignado:</strong> {proposedSfiaLevel}</p>"
            : "";

        string notesSection = !string.IsNullOrEmpty(notes)
            ? $@"<div style='background: #e7f3ff; padding: 15px; border-left: 4px solid #007bff; margin: 20px 0;'>
                    <p><strong>Comentarios del revisor:</strong></p>
                    <p>{notes}</p>
                </div>"
            : "";

        var model = new ProfileApprovedEmailModel
        {
            RecipientName = recipientName,
            ProposedSfiaLevel = proposedSfiaLevel,
            Notes = notes,
            DashboardUrl = GetDashboardUrl(),
            CompanyName = GetCompanyName(),
            SfiaLevelSection = sfiaLevelSection,
            NotesSection = notesSection,
        };

        string template = _emailTemplateService.GetProfileApprovedTemplate();

        return await SendEmailAsync(
            recipientEmail,
            recipientName,
            subject,
            template,
            model,
            cancellationToken
        );
    }

    public async Task<Result> SendInvitationNotificationAsync(
        string recipientEmail,
        string recipientName,
        string invitationLink,
        string adminEmail,
        CancellationToken cancellationToken = default
    )
    {
        string subject = "Invitación a Jala Match - Únete a nuestro equipo";
        var model = new InvitationEmailModel
        {
            RecipientName = recipientName,
            InvitationLink = invitationLink,
            AdminEmail = adminEmail,
            CompanyName = GetCompanyName(),
        };

        string template = _emailTemplateService.GetInvitationTemplate();

        return await SendEmailAsync(
            recipientEmail,
            recipientName,
            subject,
            template,
            model,
            cancellationToken
        );
    }

    public async Task<Result> SendProfileRejectedNotificationAsync(
        string recipientEmail,
        string recipientName,
        string rejectionReason,
        CancellationToken cancellationToken = default
    )
    {
        string subject = "Actualización requerida en tu perfil - Jala Match";
        var model = new ProfileRejectedEmailModel
        {
            RecipientName = recipientName,
            RejectionReason = rejectionReason,
            DashboardUrl = GetDashboardUrl(),
            CompanyName = GetCompanyName(),
        };

        string template = _emailTemplateService.GetProfileRejectedTemplate();

        return await SendEmailAsync(
            recipientEmail,
            recipientName,
            subject,
            template,
            model,
            cancellationToken
        );
    }

    public async Task<Result> SendTeamDeletedNotificationAsync(
        List<string> memberEmails,
        List<string> memberNames,
        string teamName,
        string managerEmail,
        CancellationToken cancellationToken = default
    )
    {
        var results = new List<Result>();

        for (int i = 0; i < memberEmails.Count; i++)
        {
            string memberEmail = memberEmails[i];
            string memberName = memberNames[i];

            string subject = $"Equipo disuelto: {teamName} - Jala Match";
            var model = new TeamDeletedEmailModel
            {
                RecipientName = memberName,
                TeamName = teamName,
                ManagerEmail = managerEmail,
                DashboardUrl = GetDashboardUrl(),
                CompanyName = GetCompanyName(),
            };

            string template = _emailTemplateService.GetTeamDeletedTemplate();

            Result result = await SendEmailAsync(
                memberEmail,
                memberName,
                subject,
                template,
                model,
                cancellationToken
            );

            results.Add(result);
        }

        return results.Any(r => r.IsSuccess)
            ? Result.Success()
            : Result.Failure(
                new Error(
                    "Notification.AllFailed",
                    "Failed to send notifications to all team members",
                    ErrorType.Failure
                )
            );
    }

    public async Task<Result> SendTeamMemberAddedNotificationAsync(
        string recipientEmail,
        string recipientName,
        string teamName,
        string managerEmail,
        CancellationToken cancellationToken = default
    )
    {
        string subject = $"Te han agregado al equipo: {teamName} - Jala Match";
        var model = new TeamMemberAddedEmailModel
        {
            RecipientName = recipientName,
            TeamName = teamName,
            ManagerEmail = managerEmail,
            DashboardUrl = GetDashboardUrl(),
            CompanyName = GetCompanyName(),
        };

        string template = _emailTemplateService.GetTeamMemberAddedTemplate();

        return await SendEmailAsync(
            recipientEmail,
            recipientName,
            subject,
            template,
            model,
            cancellationToken
        );
    }

    public async Task<Result> SendTeamMemberRemovedNotificationAsync(
        string recipientEmail,
        string recipientName,
        string teamName,
        string managerEmail,
        CancellationToken cancellationToken = default
    )
    {
        string subject = $"Has sido removido del equipo: {teamName} - Jala Match";
        var model = new TeamMemberRemovedEmailModel
        {
            RecipientName = recipientName,
            TeamName = teamName,
            ManagerEmail = managerEmail,
            DashboardUrl = GetDashboardUrl(),
            CompanyName = GetCompanyName(),
        };

        string template = _emailTemplateService.GetTeamMemberRemovedTemplate();

        return await SendEmailAsync(
            recipientEmail,
            recipientName,
            subject,
            template,
            model,
            cancellationToken
        );
    }

    public async Task<Result> SendTeamMemberMovedNotificationAsync(
        string recipientEmail,
        string recipientName,
        string fromTeamName,
        string toTeamName,
        string managerEmail,
        CancellationToken cancellationToken = default
    )
    {
        string subject = $"Has sido movido a un nuevo equipo - Jala Match";
        var model = new TeamMemberMovedEmailModel
        {
            RecipientName = recipientName,
            FromTeamName = fromTeamName,
            ToTeamName = toTeamName,
            ManagerEmail = managerEmail,
            DashboardUrl = GetDashboardUrl(),
            CompanyName = GetCompanyName(),
        };

        string template = _emailTemplateService.GetTeamMemberMovedTemplate();

        return await SendEmailAsync(
            recipientEmail,
            recipientName,
            subject,
            template,
            model,
            cancellationToken
        );
    }

    private async Task<Result> SendEmailAsync<T>(
        string recipientEmail,
        string recipientName,
        string subject,
        string template,
        T model,
        CancellationToken cancellationToken
    )
    {
        int attempts = 0;

        while (attempts < _maxRetryAttempts)
        {
            attempts++;

            try
            {
                // Add delay between emails to prevent SMTP overload (except for first attempt)
                if (attempts > 1)
                {
                    await Task.Delay(_retryDelayMs, cancellationToken);
                }
                else
                {
                    // Small delay even on first attempt to be gentle with SMTP server
                    await Task.Delay(_emailSendDelayMs, cancellationToken);
                }

                // Process template manually to avoid RazorLight issues
                string processedTemplate = ProcessTemplate(template, model);

                // Debug log the email configuration
                _logger.LogDebug(
                    "Sending email - From: {SenderEmail}, To: {RecipientEmail}, Subject: {Subject}",
                    _senderEmail,
                    recipientEmail,
                    subject
                );

                if (string.IsNullOrEmpty(_senderEmail))
                {
                    _logger.LogError("Sender email is empty or null");
                    return Result.Failure(
                        new Error(
                            "Notification.Configuration",
                            "Sender email not configured",
                            ErrorType.Failure
                        )
                    );
                }

                if (string.IsNullOrEmpty(recipientEmail))
                {
                    _logger.LogError("Recipient email is empty or null");
                    return Result.Failure(
                        new Error(
                            "Notification.Configuration",
                            "Recipient email is empty",
                            ErrorType.Failure
                        )
                    );
                }

                // Check if we're in development mode without email configuration
                if (string.IsNullOrEmpty(_smtpUsername) || string.IsNullOrEmpty(_smtpPassword))
                {
                    _logger.LogWarning(
                        "SMTP credentials not configured. Simulating email send for development."
                    );
                    _logger.LogInformation(
                        "📧 [SIMULATED EMAIL] To: {Email}, Subject: {Subject}",
                        recipientEmail,
                        subject
                    );
                    _logger.LogDebug("📧 [SIMULATED EMAIL] Body: {Body}", processedTemplate);
                    return Result.Success();
                }

                // Create a new SMTP client and FluentEmail instance for each send operation
                // to avoid concurrency and disposal issues
                using var smtpClient = new SmtpClient(_smtpHost)
                {
                    Port = _smtpPort,
                    Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                    EnableSsl = _enableSsl,
                    Timeout = _smtpTimeout,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                };

                var smtpSender = new SmtpSender(smtpClient);

                // Create email with manual configuration
                var email = new Email() { Sender = smtpSender };

                email
                    .SetFrom(_senderEmail, _senderName)
                    .To(recipientEmail, recipientName)
                    .Subject(subject)
                    .Body(processedTemplate, true);

                SendResponse? response = await email.SendAsync(cancellationToken);

                if (response.Successful)
                {
                    _logger.LogInformation(
                        "Email sent successfully to {Email} (attempt {Attempt})",
                        recipientEmail,
                        attempts
                    );
                    return Result.Success();
                }

                if (response.ErrorMessages != null)
                {
                    string errorMessage = string.Join("; ", response.ErrorMessages);
                    _logger.LogWarning(
                        "Failed to send email to {Email} on attempt {Attempt}: {Errors}",
                        recipientEmail,
                        attempts,
                        errorMessage
                    );

                    // If this is the last attempt, return failure
                    if (attempts >= _maxRetryAttempts)
                    {
                        _logger.LogError(
                            "Failed to send email to {Email} after {Attempts} attempts: {Errors}",
                            recipientEmail,
                            attempts,
                            errorMessage
                        );
                        return Result.Failure(
                            new Error(
                                "Notification.SendFailed",
                                $"Failed to send email after {attempts} attempts: {errorMessage}",
                                ErrorType.Failure
                            )
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Exception occurred while sending email to {Email} on attempt {Attempt}",
                    recipientEmail,
                    attempts
                );

                // If this is the last attempt, return failure
                if (attempts >= _maxRetryAttempts)
                {
                    _logger.LogError(
                        ex,
                        "Failed to send email to {Email} after {Attempts} attempts",
                        recipientEmail,
                        attempts
                    );
                    return Result.Failure(
                        new Error("Notification.Exception", ex.Message, ErrorType.Failure)
                    );
                }
            }
        }

        // This should not be reached, but just in case
        _logger.LogError(
            "Unknown error occurred while sending email to {Email} after {Attempts} attempts",
            recipientEmail,
            _maxRetryAttempts
        );
        return Result.Failure(
            new Error(
                "Notification.UnknownError",
                $"An unknown error occurred while sending email after {_maxRetryAttempts} attempts",
                ErrorType.Failure
            )
        );
    }

    private string GetDashboardUrl() =>
        _configuration["Frontend:DashboardUrl"] ?? "http://localhost:3000/my-teams";

    private string GetCompanyName() => _configuration["Company:Name"] ?? "Jala Match";

    private string ProcessTemplate<T>(string template, T model)
    {
        string processedTemplate = template;

        // Use reflection to get properties and replace placeholders
        PropertyInfo[] properties = typeof(T).GetProperties();

        foreach (PropertyInfo property in properties)
        {
            object? value = property.GetValue(model);
            string placeholder = $"@Model.{property.Name}";
            string valueString = value != null ? value.ToString() : string.Empty;

            processedTemplate = processedTemplate.Replace(placeholder, valueString);
        }

        return processedTemplate;
    }
}
