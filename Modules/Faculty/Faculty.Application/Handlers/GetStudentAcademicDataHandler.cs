using MediatR;
using Common.Application.Queries;
using Faculty.Core.Entities;
using Common.Core.Interfaces.Repsitories; 

namespace Faculty.Application.Handlers;

public class GetStudentAcademicDataHandler : IRequestHandler<GetStudentAcademicDataQuery, List<StudentAcademicRecordDto>>
{
    private readonly IBaseRepository<Enrollment> _enrollmentRepo;

    public GetStudentAcademicDataHandler(IBaseRepository<Enrollment> enrollmentRepo)
    {
        _enrollmentRepo = enrollmentRepo;
    }

    public async Task<List<StudentAcademicRecordDto>> Handle(GetStudentAcademicDataQuery request, CancellationToken ct)
    {
        var enrollments = await _enrollmentRepo.GetAsync(
            filter: e => e.Student.UserId == request.UserId,
            includeProperties: "Course,Student",
            cancellationToken: ct
        );

        return enrollments.Select(e => new StudentAcademicRecordDto(
            e.Course.Name,
            e.Grade,
            e.Course.ECTS,
            (int)e.Course.Type
        )).ToList();
    }
}