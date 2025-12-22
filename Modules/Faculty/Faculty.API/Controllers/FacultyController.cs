using Faculty.Application.DTOs;
using Faculty.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Faculty.Application.Interfaces;
using System.Collections.Generic;   



namespace Faculty.API.Controllers
{
    [ApiController]
    [Route("api/faculty/courses")]
    public class FacultyController : ControllerBase
    {
        private readonly ICourseService _service;

        public FacultyController(ICourseService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CourseDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(await _service.AddAsync(dto));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CourseDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateAsync(id, dto);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            bool ok = await _service.DeleteAsync(id);
            return ok ? Ok(new { success = true }) : NotFound();
        }
    }

    }



