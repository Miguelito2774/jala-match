using SharedKernel.Errors;

namespace Domain.Entities.Teams;

public static class TeamErrors
{
    public static Error NotFound(Guid teamId) =>
        Error.NotFound("Teams.NotFound", $"The team with ID {teamId} was not found");
        
    public static Error InvalidMember(Guid memberId) =>
        Error.Validation("Teams.InvalidMember", $"The member with ID {memberId} is not valid");
        
    public static Error InvalidMembers =>
        Error.Validation("Teams.InvalidMembers", "One or more members are invalid");
        
    public static Error CreatorNotFound(Guid creatorId) =>
        Error.Validation("Teams.CreatorNotFound", $"The creator with ID {creatorId} was not found");
}
