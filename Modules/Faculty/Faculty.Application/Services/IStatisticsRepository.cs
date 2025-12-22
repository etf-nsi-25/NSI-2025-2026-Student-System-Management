using System.Threading.Tasks;

namespace Faculty.Application.Interfaces
{
    public interface IStatisticsRepository
    {
        Task<int> GetStudentsCountAsync();
        Task<int> GetEmployeesCountAsync();
        Task<int> GetCoursesCountAsync();
        Task<int> GetActivitiesCountAsync();
    }
}
