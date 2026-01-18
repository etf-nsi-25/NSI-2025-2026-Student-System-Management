using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faculty.Application.DTOs
{
    public class StudentImportRequestDTO
    {
        public IFormFile File { get; set; }
    }
}