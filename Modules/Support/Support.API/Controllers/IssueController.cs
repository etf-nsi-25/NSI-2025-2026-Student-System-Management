using Microsoft.AspNetCore.Mvc;
using Support.Application.DTOs;
using Support.Application.Services;

namespace Support.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IssueController : ControllerBase
    {
        private readonly IIssueService _issueService;
        private readonly ILogger<IssueController> _logger;

        public IssueController(IIssueService issueService, ILogger<IssueController> logger)
        {
            _issueService = issueService ?? throw new ArgumentNullException(nameof(issueService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        [ProducesResponseType(typeof(IssueDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IssueDto>> CreateIssue([FromBody] CreateIssueDto createIssueDto, CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var issue = await _issueService.CreateIssueAsync(createIssueDto, cancellationToken);
                return CreatedAtAction(nameof(GetIssueById), new { id = issue.Id }, issue);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error while creating issue");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating issue");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while creating the issue" });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IssueDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IssueDto>> GetIssueById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var issue = await _issueService.GetIssueByIdAsync(id, cancellationToken);
                if (issue == null)
                {
                    return NotFound(new { message = $"Issue with ID {id} not found" });
                }

                return Ok(issue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving issue with ID {IssueId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving the issue" });
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<IssueDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<IssueDto>>> GetAllIssues(CancellationToken cancellationToken)
        {
            try
            {
                var issues = await _issueService.GetAllIssuesAsync(cancellationToken);
                return Ok(issues);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all issues");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving issues" });
            }
        }

        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<IssueDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<IssueDto>>> GetIssuesByUserId(string userId, CancellationToken cancellationToken)
        {
            try
            {
                var issues = await _issueService.GetIssuesByUserIdAsync(userId, cancellationToken);
                return Ok(issues);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving issues for user {UserId}", userId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving issues" });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(IssueDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IssueDto>> UpdateIssue(int id, [FromBody] UpdateIssueDto updateIssueDto, CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedIssue = await _issueService.UpdateIssueAsync(id, updateIssueDto, cancellationToken);
                if (updatedIssue == null)
                {
                    return NotFound(new { message = $"Issue with ID {id} not found" });
                }

                return Ok(updatedIssue);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error while updating issue {IssueId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating issue {IssueId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while updating the issue" });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteIssue(int id, CancellationToken cancellationToken)
        {
            try
            {
                var deleted = await _issueService.DeleteIssueAsync(id, cancellationToken);
                if (!deleted)
                {
                    return NotFound(new { message = $"Issue with ID {id} not found" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting issue {IssueId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while deleting the issue" });
            }
        }

        [HttpGet("categories")]
        [ProducesResponseType(typeof(IEnumerable<IssueCategoryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<IssueCategoryDto>>> GetAllCategories(CancellationToken cancellationToken)
        {
            try
            {
                var categories = await _issueService.GetAllCategoriesAsync(cancellationToken);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving issue categories");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving issue categories" });
            }
        }
    }
}
