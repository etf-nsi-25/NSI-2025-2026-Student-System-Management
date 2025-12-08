using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faculty.Infrastructure.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private static readonly List<Course> _data = new()
        {
            new Course
            {
                Id = Guid.NewGuid(),
                Name = "Algorithms",
                Code = "CS101",
                Type = CourseType.Mandatory,
                ProgramId = "CS",
                ECTS = 6
            },
            new Course
            {
                Id = Guid.NewGuid(),
                Name = "Databases",
                Code = "CS202",
                Type = CourseType.Mandatory,
                ProgramId = "CS",
                ECTS = 5
            }
        };

        public Task<Course> AddAsync(Course course)
        {
            course.Id = Guid.NewGuid();
            _data.Add(course);
            return Task.FromResult(course);
        }

        public Task<Course?> GetByIdAsync(Guid id)
            => Task.FromResult(_data.FirstOrDefault(x => x.Id == id));

        public Task<List<Course>> GetAllAsync()
            => Task.FromResult(_data.ToList());

        public Task<Course?> UpdateAsync(Course course)
        {
            var existing = _data.FirstOrDefault(x => x.Id == course.Id);
            if (existing == null)
                return Task.FromResult<Course?>(null);

            existing.Name = course.Name;
            existing.Code = course.Code;
            existing.Type = course.Type;
            existing.ProgramId = course.ProgramId;
            existing.ECTS = course.ECTS;

            return Task.FromResult(existing);
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            var existing = _data.FirstOrDefault(x => x.Id == id);
            if (existing == null)
                return Task.FromResult(false);

            _data.Remove(existing);
            return Task.FromResult(true);
        }
    }
}
