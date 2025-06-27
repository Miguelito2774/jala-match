using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Queries.Privacy;

public sealed record ExportUserDataQuery(Guid UserId) : IQuery<DataExportDto>;
