using Microsoft.EntityFrameworkCore;
using University.Infrastructure.Entities;

namespace University.Infrastructure.Db
{
    public class UniversityDbInitializier
    {

        public UniversityDbInitializier()
        {
        }

        public async Task SeedAsync(UniversityDbContext context)
        {
            await SeedAcademicYearsAsync(context);
            await SeedUniversityStructureAsync(context);
        }

        private async Task SeedAcademicYearsAsync(UniversityDbContext context)
        {

            if (await context.Set<AcademicYear>().AnyAsync())
                return;

            var years = new List<AcademicYear>
            {
                new AcademicYear
                {
                    Year = "2024-2025",
                    StartDate = Utc(2024, 10, 1),
                    EndDate = Utc(2025, 9, 30),
                    IsActive = false
                },
                new AcademicYear
                {
                    Year = "2025-2026",
                    StartDate = Utc(2025, 10, 1),
                    EndDate = Utc(2026, 9, 30),
                    IsActive = true
                }
            };

            await context.Set<AcademicYear>().AddRangeAsync(years);
            await context.SaveChangesAsync();
        }

        private async Task SeedUniversityStructureAsync(UniversityDbContext context)
        {
            if (await context.Set<Faculty>().AnyAsync())
                return;

            // This should be replaced with actual dean user IDs from the Identity module
            Guid placeholderDeanId = Guid.NewGuid();

            var faculties = new List<Faculty>
            {
                new Faculty
                {
                    Name = "Faculty of Electrical Engineering",
                    Code = "ETF",
                    Address = "Zmaja od Bosne bb",
                    Description = "Faculty of Electrical Engineering",
                    EstablishedDate = Utc(1970, 1, 1),
                    DeanId = placeholderDeanId,
                    Departments = new List<Department>
                    {
                        new Department
                        {
                            Name = "Computer Science",
                            Code = "RI",
                            HeadOfDepartmentId = placeholderDeanId,
                            Programs = new List<Program>
                            {
                                new Program { Name = "B.Sc. Computer Science", Code = "RI", DegreeType = "Bachelor", DurationYears = 3, Credits = 180 },
                                new Program { Name = "M.Sc. Computer Science", Code = "RI-M", DegreeType = "Master", DurationYears = 2, Credits = 120 }
                            }
                        },
                        new Department
                        {
                            Name = "Telecommunications",
                            Code = "EE",
                            HeadOfDepartmentId = placeholderDeanId,
                            Programs = new List<Program>
                            {
                                new Program { Name = "B.Sc. Telecommunications", Code = "TK", DegreeType = "Bachelor", DurationYears = 3, Credits = 180 }
                            }
                        }
                    }
                },
                new Faculty
                {
                    Name = "Faculty of Law",
                    Code = "PFSA",
                    Address = "Building B, Downtown",
                    Description = "Faculty of Law",
                    EstablishedDate = Utc(1990, 1, 1),
                    DeanId = placeholderDeanId,
                    Departments = new List<Department>
                    {
                        new Department
                        {
                            Name = "Business Administration",
                            Code = "BA",
                            HeadOfDepartmentId = placeholderDeanId,
                            Programs = new List<Program>
                            {
                                new Program { Name = "BBA", Code = "BBA-101", DegreeType = "Bachelor", DurationYears = 3, Credits = 180 },
                                new Program { Name = "MBA", Code = "MBA-500", DegreeType = "Master", DurationYears = 2, Credits = 120 }
                            }
                        }
                    }
                }
            };

            await context.Set<Faculty>().AddRangeAsync(faculties);
            await context.SaveChangesAsync();

            
        }


        private static DateTime Utc(int year, int month, int day)
            => DateTime.SpecifyKind(new DateTime(year, month, day), DateTimeKind.Utc);
    }
}