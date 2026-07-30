using System;
using System.Collections.Generic;
using System.Text;

namespace Ehmini.Application.DTOs.Quotes
{
    public class PhoenixTokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }
}
