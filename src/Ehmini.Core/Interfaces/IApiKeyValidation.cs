using System;
using System.Collections.Generic;
using System.Text;

namespace Ehmini.Core.Interfaces
{
    public interface IApiKeyValidation
    {
        bool IsValidApiKey(string apiKey);
    }
}
