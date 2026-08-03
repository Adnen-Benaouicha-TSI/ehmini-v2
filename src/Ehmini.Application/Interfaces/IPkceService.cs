using System;
using System.Collections.Generic;
using System.Text;

namespace Ehmini.Application.Interfaces
{
    public interface IPkceService
    {
        bool ValidateCodeVerifier(string codeVerifier, string codeChallenge, string method = "S256");
    }
}
