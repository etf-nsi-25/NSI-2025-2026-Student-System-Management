namespace Common.Application.Queries;

public record StudentAcademicRecordDto(
    string CourseName,
    int? Grade,
    int Ects,
    int CourseType // 1 = Mandatory, 2 = Elective
);