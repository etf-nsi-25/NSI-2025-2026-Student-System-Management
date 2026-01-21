using EventBus.Core;
using Faculty.Application.DTOs;
using Faculty.Application.Interfaces;
using Faculty.Core.Interfaces;
using Identity.Application.Interfaces;
using Identity.Core.DTO;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace Faculty.Application.Services
{
    public class StudentImportService : IStudentImportService
    {
        private readonly IUserService _userService;

        public StudentImportService(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Guid> ResolveFacultyId(ClaimsPrincipal user)
        {
            if (user?.Identity?.IsAuthenticated != true)
                throw new UnauthorizedAccessException("User is not authenticated");

            var facultyIdClaim = user.Claims.FirstOrDefault(c => c.Type == "tenantId")?.Value;

            if (!Guid.TryParse(facultyIdClaim, out var facultyId))
                throw new UnauthorizedAccessException("Invalid or missing facultyId claim");

            return facultyId;
        }

        public async Task<List<StudentImportDTO>> ConvertFile(IFormFile file, Guid facultyId)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (ext == ".csv")
                return await ParseCsv(file, facultyId);

            if (ext == ".xlsx")
                return ParseXlsx(file, facultyId);

            throw new InvalidOperationException("Unsupported file type");
        }

        public async Task ValidatePreview(List<StudentImportDTO> students)
        {
            var usernameCount = new Dictionary<string, int>();
            var emailCount = new Dictionary<string, int>();

            foreach (var s in students)
            {
                if (!string.IsNullOrWhiteSpace(s.Username))
                {
                    var key = s.Username.ToLower();
                    usernameCount[key] = usernameCount.GetValueOrDefault(key) + 1;
                }

                if (!string.IsNullOrWhiteSpace(s.Email))
                {
                    var key = s.Email.ToLower();
                    emailCount[key] = emailCount.GetValueOrDefault(key) + 1;
                }
            }

            foreach (var dto in students)
            {
                if (string.IsNullOrWhiteSpace(dto.FirstName))
                    dto.Errors.Add("First name is required");

                if (string.IsNullOrWhiteSpace(dto.LastName))
                    dto.Errors.Add("Last name is required");

                if (string.IsNullOrWhiteSpace(dto.Username))
                {
                    dto.Errors.Add("Username is required");
                }
                else if (!Regex.IsMatch(dto.Username, @"^[a-zA-Z0-9_.]+$"))
                {
                    dto.Errors.Add("Invalid username format");
                }
                else if (usernameCount[dto.Username.ToLower()] > 1)
                {
                    dto.Errors.Add("Duplicate username in file");
                }

                if (string.IsNullOrWhiteSpace(dto.IndexNumber))
                    dto.Errors.Add("Index number is required");

                if (!string.IsNullOrWhiteSpace(dto.Email))
                {
                    if (!Regex.IsMatch(dto.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    {
                        dto.Errors.Add("Invalid email format");
                    }
                    else if (emailCount[dto.Email.ToLower()] > 1)
                    {
                        dto.Errors.Add("Duplicate email in file");
                    }
                }
            }
        }

        public async Task Commit(List<StudentImportDTO> students, Guid facultyId)
        {
            var identityStudents = MapToIdentityStudents(students);

            await _userService.CreateStudentsBatch(identityStudents);
        }

        private async Task<List<StudentImportDTO>> ParseCsv(IFormFile file, Guid facultyId)
        {
            var result = new List<StudentImportDTO>();

            using var reader = new StreamReader(
                file.OpenReadStream(),
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: true
            );

            Dictionary<string, int>? headerMap = null;

            var requiredHeaders = new[]
            {
                "firstname", "lastname", "username", "email", "indexnumber"
            };

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (line == null)
                    continue;

                var cols = line.Split(',');

                if (headerMap == null)
                {
                    headerMap = cols
                        .Select((h, i) => new { Name = h.Trim().ToLower(), Index = i })
                        .ToDictionary(x => x.Name, x => x.Index);

                    foreach (var h in requiredHeaders)
                    {
                        if (!headerMap.ContainsKey(h))
                            throw new InvalidOperationException($"Missing required column: {h}");
                    }

                    continue;
                }

                var dto = new StudentImportDTO
                {
                    FirstName = Sanitize(cols.ElementAtOrDefault(headerMap["firstname"])),
                    LastName = Sanitize(cols.ElementAtOrDefault(headerMap["lastname"])),
                    Username = Sanitize(cols.ElementAtOrDefault(headerMap["username"])),
                    IndexNumber = Sanitize(cols.ElementAtOrDefault(headerMap["indexnumber"])),
                    Email = Sanitize(cols.ElementAtOrDefault(headerMap["email"])),

                    FacultyId = facultyId,
                    Role = "Student",
                    EnrollmentDate = DateTime.UtcNow,
                    Errors = new List<string>()
                };

                result.Add(dto);
            }

            return result;
        }

        private List<StudentImportDTO> ParseXlsx(IFormFile file, Guid facultyId)
        {
            var result = new List<StudentImportDTO>();

            using var stream = file.OpenReadStream();
            using var workbook = new ClosedXML.Excel.XLWorkbook(stream);

            var worksheet = workbook.Worksheets.First();

            var headerRow = worksheet.Row(1);

            var headerMap = headerRow.Cells()
                .Select((c, i) => new { Name = c.GetString().Trim().ToLower(), Index = i + 1 })
                .ToDictionary(x => x.Name, x => x.Index);

            var requiredHeaders = new[]
            {
                "firstname", "lastname", "username", "email", "indexnumber"
            };

            foreach (var h in requiredHeaders)
            {
                if (!headerMap.ContainsKey(h))
                    throw new InvalidOperationException($"Missing required column: {h}");
            }

            var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

            for (int row = 2; row <= lastRow; row++)
            {
                var r = worksheet.Row(row);

                var dto = new StudentImportDTO
                {
                    FirstName = Sanitize(r.Cell(headerMap["firstname"]).GetString()),
                    LastName = Sanitize(r.Cell(headerMap["lastname"]).GetString()),
                    Username = Sanitize(r.Cell(headerMap["username"]).GetString()),
                    Email = Sanitize(r.Cell(headerMap["email"]).GetString()),
                    IndexNumber = Sanitize(r.Cell(headerMap["indexnumber"]).GetString()),

                    FacultyId = facultyId,
                    Role = "Student",
                    EnrollmentDate = DateTime.UtcNow,
                    Errors = new List<string>()
                };

                result.Add(dto);
            }

            return result;
        }

        private static string? Sanitize(string? v)
        {
            if (string.IsNullOrWhiteSpace(v)) return v;
            v = v.Trim();
            return "=+-@".Contains(v[0]) ? "'" + v : v;
        }

        private static List<StudentImport> MapToIdentityStudents(
        IEnumerable<StudentImportDTO> students)
        {
            return students.Select(s => new Identity.Core.DTO.StudentImport
            {
                Username = s.Username,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email!,
                FacultyId = s.FacultyId,
                IndexNumber = s.IndexNumber
            }).ToList();
        }
    }
}