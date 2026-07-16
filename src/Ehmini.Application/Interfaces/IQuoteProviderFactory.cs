using System;
using System.Collections.Generic;
using System.Text;

namespace Ehmini.Application.Interfaces
{
    public interface IQuoteProviderFactory
    {
        IQuoteProvider GetActiveProvider();
    }
}
