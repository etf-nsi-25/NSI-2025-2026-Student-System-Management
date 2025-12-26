using Analytics.Application.DTOs;
using System.Threading.Tasks;

namespace Analytics.Application.Services
{
	public interface IStudentAnalyticsService
	{
		Task<StudentStatsDto> GetStudentStatsAsync(string userId, string facultyId);
	}
}