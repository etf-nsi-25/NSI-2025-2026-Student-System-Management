using MediatR;
using Analytics.Application.DTOs;
using Analytics.Application.Queries;
using Common.Application.Queries;

namespace Analytics.Application.Handlers;

public class GetStudentAnalyticsHandler : IRequestHandler<GetStudentAnalyticsQuery, StudentStatsDto>
{
    private readonly IMediator _mediator;

    public GetStudentAnalyticsHandler(IMediator mediator) => _mediator = mediator;

    public async Task<StudentStatsDto> Handle(GetStudentAnalyticsQuery request, CancellationToken ct)
    {
        var data = await _mediator.Send(new GetStudentAcademicDataQuery(request.UserId), ct);

        if (data == null || !data.Any())
            return new StudentStatsDto();

        var passedCourses = data.Where(d => d.Grade >= 6).ToList();

        double totalWeightedGrades = passedCourses.Sum(d => (d.Grade ?? 0) * d.Ects);
        int totalPassedEcts = passedCourses.Sum(d => d.Ects);
        double gpa = totalPassedEcts > 0 ? totalWeightedGrades / totalPassedEcts : 0;

        double completion = ((double)passedCourses.Count / data.Count) * 100;

        return new StudentStatsDto
        {
            Gpa = Math.Round(gpa, 2),
            TotalECTS = totalPassedEcts,
            RequiredECTS = data.Sum(d => d.Ects),
            SemesterCompletion = Math.Round(completion, 2),
            GradeDistribution = data.Select(d => new GradeEntryDto
            {
                CourseName = d.CourseName,
                Grade = d.Grade ?? 0
            }).ToList()
        };
    }
}