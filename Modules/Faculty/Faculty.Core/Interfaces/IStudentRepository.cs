using Common.Core.Interfaces.Repsitories;
using Faculty.Core.Entities;

namespace Faculty.Core.Interfaces;

public interface IStudentRepository : IBaseRepository<Student>{
    Task<Student?> GetByUserIdAsync(string userId);
}
