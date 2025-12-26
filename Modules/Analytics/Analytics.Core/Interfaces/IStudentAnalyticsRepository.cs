using System.Collections.Generic;
using System.Threading.Tasks;

namespace Analytics.Core.Interfaces
{
	public interface IStudentAnalyticsRepository
	{
		
		Task<IEnumerable<StudentCourseData>> GetStudentCoursesAsync(string userId, string facultyId);
	}

	
	public class StudentCourseData
	{
		public string CourseName { get; set; } = string.Empty;
		public int? Grade { get; set; } 
		public int Ects { get; set; }

		public bool IsPassed => Grade.HasValue && Grade.Value >= 6;
		public bool IsCompleted => Grade.HasValue;
	}
}