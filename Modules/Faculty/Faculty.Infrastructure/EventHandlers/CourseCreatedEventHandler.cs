using MediatR;
using University.Application.Events;
using Faculty.Core.Entities;
using Faculty.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

public class CourseCreatedEventHandler : INotificationHandler<CourseCreatedEvent>
{
    private readonly FacultyDbContext _db;

    public CourseCreatedEventHandler(FacultyDbContext db)
    {
        _db = db;
    }

    public async Task Handle(CourseCreatedEvent notification, CancellationToken cancellationToken)
    {
        var entity = new FacultyCourse
        {
            Id = Guid.NewGuid(),
            CourseIdFromUniversity = notification.CourseId,
            FacultyId = notification.FacultyId,
            Name = notification.Name,
            Code = notification.Code,
            Ects = notification.Ects,
            AcademicYear = notification.AcademicYear,
            Semester = notification.Semester,
            ProfessorId = notification.ProfessorId
        };

        _db.FacultyCourses.Add(entity);
        await _db.SaveChangesAsync();
    }
}
