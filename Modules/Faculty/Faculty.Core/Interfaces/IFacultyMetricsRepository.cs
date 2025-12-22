namespace Faculty.Core.Interfaces
{
    public interface IFacultyMetricsRepository
    {
        Task<int> GetStudentsCountAsync();
        Task<int> GetEmployeesCountAsync();
        Task<int> GetCoursesCountAsync();
        Task<int> GetActivitiesCountAsync();
    }
}
