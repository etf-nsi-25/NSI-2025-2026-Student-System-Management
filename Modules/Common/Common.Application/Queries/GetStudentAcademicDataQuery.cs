using MediatR;

namespace Common.Application.Queries;

public record GetStudentAcademicDataQuery(string UserId) : IRequest<List<StudentAcademicRecordDto>>;