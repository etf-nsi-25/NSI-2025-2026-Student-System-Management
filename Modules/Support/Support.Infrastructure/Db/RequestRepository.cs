using Support.Core.Entities;
using Support.Core.Interfaces;
using Common.Infrastructure.Repositories;

namespace Support.Infrastructure.Db
{
    public class RequestRepository : BaseRepository<DocumentRequest>, IRequestRepository
    {
        public RequestRepository(SupportDbContext context) : base(context)
        {
        }

        public async Task<DocumentRequest?> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        public async Task<DocumentRequest> CreateAsync(DocumentRequest request)
        {
            return await base.AddAsync(request);
        }
    }
}
