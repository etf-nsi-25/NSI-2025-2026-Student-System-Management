using Faculty.Application.DTOs;

namespace Faculty.Application.Interfaces;

/// <summary>
/// Service interface for retrieving upcoming activities for professors.
/// </summary>
public interface IUpcomingActivityService
{
    Task<List<UpcomingActivityDTO>> GetUpcomingActivitiesAsync(string userId);
}
