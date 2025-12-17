using Support.Application.DTOs;

namespace Support.Application.Services
{
    public interface IIssueService
    {
        Task<IssueDto> CreateIssueAsync(CreateIssueDto createIssueDto, CancellationToken cancellationToken = default);
        Task<IssueDto?> GetIssueByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<IssueDto>> GetAllIssuesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<IssueDto>> GetIssuesByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<IssueDto?> UpdateIssueAsync(int id, UpdateIssueDto updateIssueDto, CancellationToken cancellationToken = default);
        Task<bool> DeleteIssueAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);
    }
}
