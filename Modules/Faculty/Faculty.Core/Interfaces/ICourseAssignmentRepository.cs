using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Faculty.Core.Entities;
using global::Faculty.Core.Entities;
using System;
using System.Threading.Tasks;

namespace Faculty.Core.Interfaces
{
   

    namespace Faculty.Core.Interfaces
    {
        public interface ICourseAssignmentRepository
        {
            Task<Teacher?> GetTeacherForCourseAsync(Guid courseId);
        }
    }

}
