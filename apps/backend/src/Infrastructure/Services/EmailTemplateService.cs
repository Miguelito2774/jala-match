using Infrastructure.Services.Templates;

namespace Infrastructure.Services;

public interface IEmailTemplateService
{
    string GetProfileApprovedTemplate();
    string GetProfileRejectedTemplate();
    string GetTeamDeletedTemplate();
    string GetTeamMemberAddedTemplate();
    string GetTeamMemberRemovedTemplate();
    string GetTeamMemberMovedTemplate();
    string GetInvitationTemplate();
    string GetPasswordResetTemplate();
}

public sealed class EmailTemplateService : IEmailTemplateService
{
    public string GetProfileApprovedTemplate() => EmailTemplates.ProfileApproved;
    public string GetProfileRejectedTemplate() => EmailTemplates.ProfileRejected;
    public string GetTeamDeletedTemplate() => EmailTemplates.TeamDeleted;
    public string GetTeamMemberAddedTemplate() => EmailTemplates.TeamMemberAdded;
    public string GetTeamMemberRemovedTemplate() => EmailTemplates.TeamMemberRemoved;
    public string GetTeamMemberMovedTemplate() => EmailTemplates.TeamMemberMoved;
    public string GetInvitationTemplate() => EmailTemplates.Invitation;
    public string GetPasswordResetTemplate() => EmailTemplates.PasswordReset;
}
