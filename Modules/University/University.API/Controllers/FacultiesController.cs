using Microsoft.AspNetCore.Mvc;
using University.Application.DTOs;
using University.Application.Interfaces.Services;

namespace University.API.Controllers
{
    [ApiController]
    [Route("api/university/faculties")]
    public class FacultiesController : ControllerBase
    {
        private readonly IFacultyService _facultyService;

        public FacultiesController(IFacultyService facultyService)
        {
            _facultyService = facultyService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FacultyDto>>> GetFaculties()
        {
            var faculties = await _facultyService.GetAllFacultiesAsync();
            return Ok(faculties);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FacultyDto>> GetFaculty(int id)
        {
            var faculty = await _facultyService.GetFacultyByIdAsync(id);
            if (faculty == null)
            {
                return NotFound();
            }
            return Ok(faculty);
        }

        [HttpPost]
        public async Task<ActionResult<FacultyDto>> PostFaculty(CreateFacultyDto createFacultyDto)
        {
            var newFaculty = await _facultyService.CreateFacultyAsync(createFacultyDto);
            return CreatedAtAction(nameof(GetFaculty), new { id = newFaculty.Id }, newFaculty);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutFaculty(int id, UpdateFacultyDto updateFacultyDto)
        {
            var updatedFaculty = await _facultyService.UpdateFacultyAsync(id, updateFacultyDto);
            if (updatedFaculty == null)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFaculty(int id)
        {
            var result = await _facultyService.DeleteFacultyAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
