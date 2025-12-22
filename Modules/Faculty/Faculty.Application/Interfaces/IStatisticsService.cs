namespace Faculty.Application.Interfaces
{
    public interface IStatisticsService
    {
        Task<int> GetStudentsCountAsync();
        Task<int> GetEmployeesCountAsync();
        Task<int> GetCoursesCountAsync();
        Task<int> GetActivitiesCountAsync();
    }
}
