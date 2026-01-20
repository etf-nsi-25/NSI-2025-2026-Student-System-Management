using System;

namespace Support.Core.Entities
{
    public class EnrollmentRequest
    {
        public Guid Id { get; set; }

        public string UserId { get; set; } = default!;
        public int FacultyId { get; set; }

        public string AcademicYear { get; set; } = default!;
        public int Semester { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; }
        public DateTime? DecisionAt { get; set; }
        public string? DecidedByUserId { get; set; }
        public string? DecisionNote { get; set; }
    }
}
