using Application.Abstractions.Messaging;
using MediatR;
using SharedKernel.Results;

namespace Application.Commands.Privacy;

public sealed record CancelDataDeletionRequestCommand(
    Guid UserId,
    Guid RequestId,
    string? IpAddress,
    string? UserAgent
) : ICommand;
