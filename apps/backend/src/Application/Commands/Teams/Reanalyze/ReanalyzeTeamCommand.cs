using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Commands.Teams.Reanalyze;

public sealed record ReanalyzeTeamCommand(Guid TeamId, List<Guid> MemberIds, Guid LeaderId)
    : ICommand<AiServiceResponse>;
