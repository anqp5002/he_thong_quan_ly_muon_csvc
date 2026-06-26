using System;
using System.Threading.Tasks;
using CSVC_PTIT.Core.Interfaces;

namespace CSVC_PTIT.Core.Services;

public class EmailService : IEmailService
{
    public Task SendEmailAsync(string toEmail, string subject, string body)
    {
        // Mock email sending: Just print to Console and Debug log
        Console.WriteLine($"[EMAIL SENT TO {toEmail}]");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine($"Body: {body}");
        Console.WriteLine("-------------------------------------------------");
        
        System.Diagnostics.Debug.WriteLine($"[EMAIL SENT TO {toEmail}] Subject: {subject}");
        
        return Task.CompletedTask;
    }
}
