using Support.Application.DTOs;
using Support.Core.Entities;
using Support.Core.Interfaces.Repositories;

namespace Support.Application.Services
{
    public class IssueService : IIssueService
    {
        private readonly IIssueRepository _issueRepository;
        private readonly ICategoryRepository _categoryRepository;

        public IssueService(IIssueRepository issueRepository, ICategoryRepository categoryRepository)
        {
            _issueRepository = issueRepository ?? throw new ArgumentNullException(nameof(issueRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        public async Task<IssueDto> CreateIssueAsync(CreateIssueDto createIssueDto, CancellationToken cancellationToken = default)
        {
            // Validate that the category exists
            var categoryExists = await _categoryRepository.AnyAsync(c => c.Id == createIssueDto.CategoryId, cancellationToken);
            if (!categoryExists)
            {
                throw new ArgumentException($"Category with ID {createIssueDto.CategoryId} does not exist.");
            }

            var issue = new Issue
            {
                Subject = createIssueDto.Subject,
                Description = createIssueDto.Description,
                CategoryId = createIssueDto.CategoryId,
                UserId = createIssueDto.UserId,
                Status = Status.Open,
                CreatedAt = DateTime.UtcNow
            };

            var createdIssue = await _issueRepository.AddAsync(issue, cancellationToken);

            return await MapToIssueDtoAsync(createdIssue, cancellationToken);
        }

        public async Task<IssueDto?> GetIssueByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var issues = await _issueRepository.GetAsync(
                filter: i => i.Id == id,
                includeProperties: "Category",
                cancellationToken: cancellationToken
            );

            var issue = issues.FirstOrDefault();
            return issue != null ? MapToIssueDto(issue) : null;
        }

        public async Task<IEnumerable<IssueDto>> GetAllIssuesAsync(CancellationToken cancellationToken = default)
        {
            var issues = await _issueRepository.GetAsync(
                includeProperties: "Category",
                orderBy: q => q.OrderByDescending(i => i.CreatedAt),
                cancellationToken: cancellationToken
            );

            return issues.Select(MapToIssueDto);
        }

        public async Task<IEnumerable<IssueDto>> GetIssuesByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var issues = await _issueRepository.GetAsync(
                filter: i => i.UserId == userId,
                includeProperties: "Category",
                orderBy: q => q.OrderByDescending(i => i.CreatedAt),
                cancellationToken: cancellationToken
            );

            return issues.Select(MapToIssueDto);
        }

        public async Task<IssueDto?> UpdateIssueAsync(int id, UpdateIssueDto updateIssueDto, CancellationToken cancellationToken = default)
        {
            var issue = await _issueRepository.GetByIdAsync(id, cancellationToken);
            if (issue == null)
            {
                return null;
            }

            // Update only provided fields
            if (!string.IsNullOrEmpty(updateIssueDto.Subject))
            {
                issue.Subject = updateIssueDto.Subject;
            }

            if (!string.IsNullOrEmpty(updateIssueDto.Description))
            {
                issue.Description = updateIssueDto.Description;
            }

            if (updateIssueDto.CategoryId.HasValue)
            {
                var categoryExists = await _categoryRepository.AnyAsync(c => c.Id == updateIssueDto.CategoryId.Value, cancellationToken);
                if (!categoryExists)
                {
                    throw new ArgumentException($"Category with ID {updateIssueDto.CategoryId.Value} does not exist.");
                }
                issue.CategoryId = updateIssueDto.CategoryId.Value;
            }

            if (updateIssueDto.Status.HasValue)
            {
                issue.Status = (Status)updateIssueDto.Status.Value;

                // If status is closed, set ClosedAt
                if (issue.Status == Status.Closed && !issue.ClosedAt.HasValue)
                {
                    issue.ClosedAt = DateTime.UtcNow;
                }
            }

            await _issueRepository.UpdateAsync(issue, cancellationToken);

            return await MapToIssueDtoAsync(issue, cancellationToken);
        }

        public async Task<bool> DeleteIssueAsync(int id, CancellationToken cancellationToken = default)
        {
            var issue = await _issueRepository.GetByIdAsync(id, cancellationToken);
            if (issue == null)
            {
                return false;
            }

            await _issueRepository.DeleteAsync(issue, cancellationToken);
            return true;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
        {
            var categories = await _categoryRepository.GetAsync(
                orderBy: q => q.OrderBy(c => c.Priority).ThenBy(c => c.Title),
                cancellationToken: cancellationToken
            );

            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Title = c.Title,
                Priority = c.Priority
            });
        }

        private IssueDto MapToIssueDto(Issue issue)
        {
            return new IssueDto
            {
                Id = issue.Id,
                Subject = issue.Subject,
                Description = issue.Description,
                CategoryId = issue.CategoryId,
                CategoryTitle = issue.Category?.Title ?? string.Empty,
                UserId = issue.UserId,
                Status = issue.Status.ToString(),
                CreatedAt = issue.CreatedAt,
                ClosedAt = issue.ClosedAt
            };
        }

        private async Task<IssueDto> MapToIssueDtoAsync(Issue issue, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(issue.CategoryId, cancellationToken);

            return new IssueDto
            {
                Id = issue.Id,
                Subject = issue.Subject,
                Description = issue.Description,
                CategoryId = issue.CategoryId,
                CategoryTitle = category?.Title ?? string.Empty,
                UserId = issue.UserId,
                Status = issue.Status.ToString(),
                CreatedAt = issue.CreatedAt,
                ClosedAt = issue.ClosedAt
            };
        }
    }
}
