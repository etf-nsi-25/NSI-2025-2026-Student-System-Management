using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using University.Application.DTOs;
using University.Application.Interfaces;

namespace University.API.Controllers
{
    [ApiController]
    [Route("api/university/faculties")]
    public class FacultyController : ControllerBase
    {
        private readonly IFacultyService _facultyService;

        public FacultyController(IFacultyService facultyService)
        {
            _facultyService = facultyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFaculties([FromQuery] string? name = null)
        {
            try
            {
                var faculties = await _facultyService.GetAllFacultiesAsync(name);
                return Ok(faculties);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving faculties.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFacultyById(int id)
        {
            try
            {
                var faculty = await _facultyService.GetFacultyByIdAsync(id);
                if (faculty == null)
                {
                    return NotFound();
                }
                return Ok(faculty);
            }
            catch (Exception)
            {
                return StatusCode(500, $"An error occurred while retrieving the faculty with id {id}.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateFaculty([FromBody] CreateFacultyDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newFaculty = await _facultyService.CreateFacultyAsync(dto);
                return CreatedAtAction(nameof(GetFacultyById), new { id = newFaculty.Id }, newFaculty);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while creating the faculty.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFaculty(int id, [FromBody] UpdateFacultyDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedFaculty = await _facultyService.UpdateFacultyAsync(id, dto);
                if (updatedFaculty == null)
                {
                    return NotFound();
                }
                return Ok(updatedFaculty);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, $"An error occurred while updating the faculty with id {id}.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFaculty(int id)
        {
            try
            {
                var result = await _facultyService.DeleteFacultyAsync(id);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, $"An error occurred while deleting the faculty with id {id}.");
            }
        }
    }
}