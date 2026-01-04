using Microsoft.AspNetCore.Mvc;
using Support.Application.DTOs;
using Support.Application.Services;

namespace Support.API.Controllers
{
    [ApiController]
    [Route("api/issue-category")]
    public class IssueCategoryController : ControllerBase
    {
        private readonly IIssueService _issueService;
        private readonly ILogger<IssueCategoryController> _logger;

        public IssueCategoryController(IIssueService issueService, ILogger<IssueCategoryController> logger)
        {
            _issueService = issueService ?? throw new ArgumentNullException(nameof(issueService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        [ProducesResponseType(typeof(IssueCategoryDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IssueCategoryDto>> CreateCategory([FromBody] CreateIssueCategoryDto createCategoryDto, CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var category = await _issueService.CreateCategoryAsync(createCategoryDto, cancellationToken);
                return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while creating the category" });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IssueCategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IssueCategoryDto>> GetCategoryById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var category = await _issueService.GetCategoryByIdAsync(id, cancellationToken);
                if (category == null)
                    return NotFound(new { message = $"Category with ID {id} not found" });

                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving category with ID {CategoryId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving the category" });
            }
        }

        [HttpGet]
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
                _logger.LogError(ex, "Error retrieving all categories");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving categories" });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(IssueCategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IssueCategoryDto>> UpdateCategory(int id, [FromBody] UpdateIssueCategoryDto updateCategoryDto, CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var updatedCategory = await _issueService.UpdateCategoryAsync(id, updateCategoryDto, cancellationToken);
                if (updatedCategory == null)
                    return NotFound(new { message = $"Category with ID {id} not found" });

                return Ok(updatedCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category {CategoryId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while updating the category" });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteCategory(int id, CancellationToken cancellationToken)
        {
            try
            {
                var deleted = await _issueService.DeleteCategoryAsync(id, cancellationToken);
                if (!deleted)
                    return NotFound(new { message = $"Category with ID {id} not found" });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Cannot delete category {CategoryId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category {CategoryId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while deleting the category" });
            }
        }
    }
}
