using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task<Request> CreateAsync(Request request)
        {
            _db.Requests.Add(request);
            await _db.SaveChangesAsync();
            return request;
        }

        public async Task<Request?> GetByIdAsync(int id)
        {
            return await _db.Requests.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}

