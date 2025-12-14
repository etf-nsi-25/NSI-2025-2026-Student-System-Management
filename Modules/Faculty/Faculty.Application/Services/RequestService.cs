using Faculty.Application.DTOs;
using Faculty.Application.Interfaces; 

public class RequestService : IRequestService
{

    public async Task<IEnumerable<StudentRequestDto>> GetAllRequestsAsync()
    {
        // Mock data
        return new List<StudentRequestDto>
        {
            new StudentRequestDto 
        { 
            Id = Guid.NewGuid(), 
            Date = DateTime.Now.AddDays(-1), 
            StudentIndex = "18001/2024", 
            RequestType = "Certificate", 
            RequestDetails = "Request for study status certificate.", 
            Status = "Pending" 
        },
        new StudentRequestDto 
        { 
            Id = Guid.NewGuid(), 
            Date = DateTime.Now.AddDays(-3), 
            StudentIndex = "19005/2023", 
            RequestType = "Grade Transfer", 
            RequestDetails = "Request for grade transfer.", 
            Status = "Completed" 
        },
        
        new StudentRequestDto 
        { 
            Id = Guid.NewGuid(), 
            Date = DateTime.Now.AddDays(-2), 
            StudentIndex = "20012/2024", 
            RequestType = "Certificate", 
            RequestDetails = "Request for enrollment certificate.", 
            Status = "Pending" 
        },
        new StudentRequestDto 
        { 
            Id = Guid.NewGuid(), 
            Date = DateTime.Now.AddDays(-4), 
            StudentIndex = "19543/2023", 
            RequestType = "Grade Transfer", 
            RequestDetails = "Application for grade transfer to another faculty.", 
            Status = "Pending" 
        },
        new StudentRequestDto 
        { 
            Id = Guid.NewGuid(), 
            Date = DateTime.Now.AddDays(-6), 
            StudentIndex = "18567/2022", 
            RequestType = "Certificate", 
            RequestDetails = "Certificate needed for visa application.", 
            Status = "Completed" 
        },
        new StudentRequestDto 
        { 
            Id = Guid.NewGuid(), 
            Date = DateTime.Now.AddDays(-8), 
            StudentIndex = "21034/2024", 
            RequestType = "Grade Transfer", 
            RequestDetails = "Request to transfer grades from previous studies.", 
            Status = "Rejected" 
        },
        new StudentRequestDto 
        { 
            Id = Guid.NewGuid(), 
            Date = DateTime.Now.AddDays(-10), 
            StudentIndex = "19245/2023", 
            RequestType = "Certificate", 
            RequestDetails = "Certificate for scholarship application.", 
            Status = "Pending" 
        },
        new StudentRequestDto 
        { 
            Id = Guid.NewGuid(), 
            Date = DateTime.Now.AddDays(-12), 
            StudentIndex = "17896/2022", 
            RequestType = "Grade Transfer", 
            RequestDetails = "Grade transfer request for master program.", 
            Status = "Completed" 
        }
        };
    }

    public async Task<StudentRequestDto> ProcessRequestAsync(Guid requestId, CreateConfirmationRequest request)
    {
        return new StudentRequestDto
        {
            Id = requestId,
            Date = DateTime.Now,
            StudentIndex = request.StudentIndex,
            RequestType = request.RequestType,
            RequestDetails = "Request processed.",
            Status = request.StatusRequest == "Approved" ? "Completed" : "Rejected"
        };
    }
}