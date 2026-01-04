using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using Identity.Core.Enums;
using Identity.Core.Events;

namespace Faculty.Application.Services;

public class StudentService(IStudentRepository repository)
{
    public async Task HandleStudentCreated(UserCreatedEvent userCreatedEvent, CancellationToken ct)
    {
        if (userCreatedEvent.Role == UserRole.Student)
        {
            await repository.AddAsync(new Student
                {
                    UserId = userCreatedEvent.UserId.ToString(),
                    IndexNumber = userCreatedEvent.IndexNumber!,
                    LastName = userCreatedEvent.LastName,
                    FirstName = userCreatedEvent.FirstName,
                    EnrollmentDate = DateTime.UtcNow, // TODO: probably not NOW, this is just an example
                    CreatedAt = DateTime.UtcNow
                },
                ct
            );
        }
    }
}
