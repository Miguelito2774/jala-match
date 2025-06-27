using Application.Abstractions.Messaging;
using Application.DTOs;
using MediatR;
using SharedKernel.Results;

namespace Application.Commands.Privacy;

public sealed record UpdateConsentCommand(Guid UserId, UpdateConsentRequestDto Request) : ICommand;
