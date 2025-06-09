namespace Infrastructure.Services;

public class ProfileApprovedEmailModel
{
    public string RecipientName { get; set; } = string.Empty;
    public int? ProposedSfiaLevel { get; set; }
    public string? Notes { get; set; }
    public string DashboardUrl { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string SfiaLevelSection { get; set; } = string.Empty;
    public string NotesSection { get; set; } = string.Empty;
}

public class ProfileRejectedEmailModel
{
    public string RecipientName { get; set; } = string.Empty;
    public string RejectionReason { get; set; } = string.Empty;
    public string DashboardUrl { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
}

public class TeamDeletedEmailModel
{
    public string RecipientName { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public string ManagerEmail { get; set; } = string.Empty;
    public string DashboardUrl { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
}

public class TeamMemberAddedEmailModel
{
    public string RecipientName { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public string ManagerEmail { get; set; } = string.Empty;
    public string DashboardUrl { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
}

public class TeamMemberRemovedEmailModel
{
    public string RecipientName { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public string ManagerEmail { get; set; } = string.Empty;
    public string DashboardUrl { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
}

public class TeamMemberMovedEmailModel
{
    public string RecipientName { get; set; } = string.Empty;
    public string FromTeamName { get; set; } = string.Empty;
    public string ToTeamName { get; set; } = string.Empty;
    public string ManagerEmail { get; set; } = string.Empty;
    public string DashboardUrl { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
}

public class InvitationEmailModel
{
    public string RecipientName { get; set; } = string.Empty;
    public string InvitationLink { get; set; } = string.Empty;
    public string AdminEmail { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
}

public class PasswordResetEmailModel
{
    public string RecipientName { get; set; } = string.Empty;
    public string ResetLink { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
}
