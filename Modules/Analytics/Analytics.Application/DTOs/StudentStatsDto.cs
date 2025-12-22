using System;
using System.Collections.Generic;

namespace Analytics.Application.DTOs
{
	public class StudentStatsDto
	{
		public double Gpa { get; set; }

		public int TotalECTS { get; set; }

		public int RequiredECTS { get; set; }

		public double SemesterCompletion { get; set; }

		public List<GradeEntryDto> GradeDistribution { get; set; } = new();
	}

	public class GradeEntryDto
	{
		public string CourseName { get; set; } = string.Empty;
		public int Grade { get; set; }
	}
}