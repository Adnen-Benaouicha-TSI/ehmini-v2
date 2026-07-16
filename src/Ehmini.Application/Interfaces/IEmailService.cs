using System.Threading.Tasks;

namespace Ehmini.Application.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string receiver, string subject, string message);
}