using MediatR;
using Analytics.Application.DTOs;

namespace Analytics.Application.Queries;

public record GetStudentAnalyticsQuery(string UserId) : IRequest<StudentStatsDto>;