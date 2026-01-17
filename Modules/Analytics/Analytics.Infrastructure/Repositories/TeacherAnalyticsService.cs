using Analytics.Application.Interfaces;
using Analytics.Application.DTO;
using Analytics.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace Analytics.Infrastructure.Repositories;

public class TeacherAnalyticsService : ITeacherAnalyticsService
{
    private readonly AnalyticsDbContext _context;

    public TeacherAnalyticsService(AnalyticsDbContext context)
    {
        _context = context;
    }

    public async Task<TeacherFilterDataDto> GetTeacherFilterDataByEmailAsync(string email)
    {
        
        var teacherId = await _context.Database
            .SqlQueryRaw<int>(@"SELECT ""Id"" AS ""Value"" FROM ""Teacher"" WHERE ""Email"" = {0} LIMIT 1", email)
            .ToListAsync();

        var id = teacherId.FirstOrDefault();

        if (id == 0) return null;

        return await GetTeacherFilterDataAsync(id);
    }

    
    public async Task<TeacherFilterDataDto> GetTeacherFilterDataByUserIdAsync(Guid userId)
    {
        var teacherId = await _context.Database
            .SqlQueryRaw<int>(@"SELECT ""Id"" FROM ""Teacher"" WHERE ""UserId"" = {0} LIMIT 1", userId)
            .FirstOrDefaultAsync();

        if (teacherId == 0) 
        {
            return null;
        }

        
        return await GetTeacherFilterDataAsync(teacherId);
    }

    public async Task<TeacherFilterDataDto> GetTeacherFilterDataAsync(int teacherId)
    {
        var filterData = new TeacherFilterDataDto();

        var coursesSql = @"
            SELECT DISTINCT c.""Name""
            FROM ""CourseAssignment"" ca
            JOIN ""Course"" c ON ca.""CourseId"" = c.""Id""
            WHERE ca.""TeacherId"" = {0}";

        filterData.Courses = await _context.Database
            .SqlQueryRaw<string>(coursesSql, teacherId)
            .ToListAsync();

        
        var yearsSql = @"
        SELECT ""Year""
        FROM ""AcademicYears""
        ORDER BY ""StartDate"" DESC";

        filterData.Years = await _context.Database
            .SqlQueryRaw<string>(yearsSql)
            .ToListAsync();

        return filterData;
    }
}