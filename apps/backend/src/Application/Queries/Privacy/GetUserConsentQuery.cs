using Application.Abstractions.Messaging;
using Application.DTOs;
using MediatR;
using SharedKernel.Results;

namespace Application.Queries.Privacy;

public sealed record GetUserConsentQuery(Guid UserId) : IQuery<ConsentSettingsDto>;
