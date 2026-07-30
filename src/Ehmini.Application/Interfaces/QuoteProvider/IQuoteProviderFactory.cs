
namespace Ehmini.Application.Interfaces.QuoteProvider
{
    public interface IQuoteProviderFactory
    {
        IQuoteProvider GetActiveProvider();
    }
}
