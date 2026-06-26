using System.Threading.Tasks;

namespace CSVC_PTIT.Core.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body);
}
