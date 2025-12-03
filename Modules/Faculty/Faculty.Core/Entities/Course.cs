<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faculty.Core.Entities
{
    public enum CourseType
    {
        Mandatory = 1,
        Elective = 2
    }
    public class Course
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public CourseType Type { get; set; }
        public string ProgramId { get; set; }
        public int ECTS { get; set; }
    }
}
=======
using Faculty.Core.Interfaces;

namespace Faculty.Core.Entities;

/// <summary>
/// Represents a course in the faculty system.
/// </summary>
public class Course : ITenantAware
{
    public int Id { get; set; }
    public Guid FacultyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public CourseType Type { get; set; }
    public string? ProgramId { get; set; }
    public int? ECTS { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<CourseAssignment> CourseAssignments { get; set; } = new List<CourseAssignment>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    public ICollection<Exam> Exams { get; set; } = new List<Exam>();
    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
}

/// <summary>
/// Enum representing the type of course.
/// </summary>
public enum CourseType
{
    Lecture,
    Seminar,
    Lab,
    Project,
    Other
}

>>>>>>> c4e5064 (PBI-301: Implement Faculty database and multi-tenancy infrastructure)
