using Faculty.Application.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Faculty.Application.Interfaces
{
    public interface IStudentImportService
    {
        Task<Guid> ResolveFacultyId(ClaimsPrincipal user);
        Task<List<StudentImportDTO>> ConvertFile(IFormFile file, Guid facultyId);
        Task ValidatePreview(List<StudentImportDTO> students);
        Task Commit(List<StudentImportDTO> students, Guid facultyId);
    }
}