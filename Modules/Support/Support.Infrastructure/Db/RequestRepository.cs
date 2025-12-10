using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Support.Core.Entities;
using Support.Core.Interfaces;

namespace Support.Infrastructure.Db
{
    public class RequestRepository : IRequestRepository
    {
        private readonly SupportDbContext _db;

        public RequestRepository(SupportDbContext db)
        {
            _db = db;
        }

        public async Task<DocumentRequest> CreateAsync(DocumentRequest request)
        {
            _db.DocumentRequests.Add(request);
            await _db.SaveChangesAsync();
            return request;
        }

        public async Task<DocumentRequest?> GetByIdAsync(int id)
        {
            return await _db.DocumentRequests
                            .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
