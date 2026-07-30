

namespace Ehmini.Application.Interfaces.PersonProviderService
{
    public interface IPersonProviderFactory
    {
        IPersonProvider GetActiveProvider();
    }
}
