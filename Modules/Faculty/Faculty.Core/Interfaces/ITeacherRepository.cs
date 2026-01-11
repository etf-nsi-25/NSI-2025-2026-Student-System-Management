using Faculty.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faculty.Core.Interfaces
{
    public interface ITeacherRepository
    {
        Task<Teacher?> GetByUserIdAsync(string userId);
        Task<Teacher?> GetByIdAsync(int id);
    }
}