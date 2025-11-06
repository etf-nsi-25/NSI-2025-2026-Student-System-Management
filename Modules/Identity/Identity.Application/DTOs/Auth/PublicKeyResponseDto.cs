using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.DTOs.Auth;

public class PublicKeyResponseDto
    {
        public string PublicKey { get; set; } = string.Empty;
        public string Algorithm { get; set; } = "RS256";
        public string KeyId { get; set; } = string.Empty;
    }

