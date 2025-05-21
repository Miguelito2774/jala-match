﻿using Application.Commands.Teams.Create;
using Application.DTOs;
using SharedKernel.Results;

namespace Application.Abstractions.Services;

public interface ITeamService
{
    Task<Result<AiServiceResponse>> GenerateTeams(
        Guid creatorId,
        List<TeamRequirements> requirements,
        int sfiaLevel,
        int teamSize,
        List<string> technologies,
        WeightCriteria weights,
        CancellationToken cancellationToken,
        bool availability = true
    );

    Task<Result<TeamResponse>> CreateTeam(
        CreateTeamCommand command,
        CancellationToken cancellationToken
    );

    Task<Result<List<TeamMemberRecommendation>>> FindTeamMembers(
        FindTeamMemberRequest request,
        CancellationToken cancellationToken
    );

    Task<Result<TeamResponse>> AddTeamMember(
        TeamMemberUpdateRequest request,
        CancellationToken cancellationToken
    );
}
