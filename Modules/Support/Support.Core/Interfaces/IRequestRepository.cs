using System.Threading.Tasks;
using Support.Core.Entities;

namespace Support.Core.Interfaces
{
    public interface IRequestRepository
    {
        Task<DocumentRequest?> GetByIdAsync(int id);
        Task<DocumentRequest> CreateAsync(DocumentRequest request);
    }
}
