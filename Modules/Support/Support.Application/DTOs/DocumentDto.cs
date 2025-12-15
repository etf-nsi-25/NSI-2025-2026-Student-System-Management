using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Support.Application.DTOs
{
    public record DocumentDto(int Id, string FileName, string Description, int StudentId);
}