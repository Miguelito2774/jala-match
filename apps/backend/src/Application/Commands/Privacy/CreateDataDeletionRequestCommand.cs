using Application.Abstractions.Messaging;
using Application.DTOs;
using MediatR;
using SharedKernel.Results;

namespace Application.Commands.Privacy;

public sealed record CreateDataDeletionRequestCommand(
    Guid UserId,
    DataDeletionRequestDto Request,
    string? IpAddress,
    string? UserAgent
) : ICommand<DataDeletionResponseDto>;
