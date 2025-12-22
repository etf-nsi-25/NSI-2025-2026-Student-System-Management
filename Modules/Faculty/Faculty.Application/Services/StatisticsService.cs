using Faculty.Application.Interfaces;

namespace Faculty.Application.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IStatisticsRepository _repository;

        public StatisticsService(IStatisticsRepository repository)
        {
            _repository = repository;
        }

        public Task<int> GetStudentsCountAsync() => _repository.GetStudentsCountAsync();
        public Task<int> GetEmployeesCountAsync() => _repository.GetEmployeesCountAsync();
        public Task<int> GetCoursesCountAsync() => _repository.GetCoursesCountAsync();
        public Task<int> GetActivitiesCountAsync() => _repository.GetActivitiesCountAsync();
    }
}
