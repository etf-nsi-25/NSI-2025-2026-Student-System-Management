using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Support.Core.Entities;

namespace Support.Core.Interfaces
{
    public interface IRequestRepository
    {
        Task<Request?> GetByIdAsync(int id);
        Task<Request> CreateAsync(Request request);
    }
}

