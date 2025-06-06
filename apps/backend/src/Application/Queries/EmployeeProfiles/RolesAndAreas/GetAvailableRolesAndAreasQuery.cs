using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Queries.EmployeeProfiles.RolesAndAreas;

public sealed record GetAvailableRolesAndAreasQuery : IQuery<AvailableRolesAndAreasResponse>;
