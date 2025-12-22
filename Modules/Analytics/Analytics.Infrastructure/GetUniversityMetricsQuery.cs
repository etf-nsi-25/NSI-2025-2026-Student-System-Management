using MediatR;

namespace Analytics.Infrastructure
{
    // Query za MediatR
    public class GetUniversityMetricsQuery : IRequest<UniversityMetricsDto>
    {
        // Ako nema parametara, ostavi prazno
    }

    // DTO koji se vraća frontend-u
    public class UniversityMetricsDto
    {
        public int StudentsCount { get; set; }
        public int EmployeesCount { get; set; }
        public int CoursesCount { get; set; }
        public int ActivityCount { get; set; }
    }
}
