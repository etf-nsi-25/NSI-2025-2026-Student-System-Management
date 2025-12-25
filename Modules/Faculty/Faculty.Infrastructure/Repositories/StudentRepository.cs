using Common.Infrastructure.Repositories;
using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using Faculty.Infrastructure.Db;

namespace Faculty.Infrastructure.Repositories;

public class StudentRepository(FacultyDbContext context) : BaseRepository<Student>(context), IStudentRepository;
