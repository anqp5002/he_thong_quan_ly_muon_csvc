using System;
using System.Threading.Tasks;
using CSVC_PTIT.Core.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace CSVC_PTIT.Core.Services;

public class EmailService : IEmailService
{
    // FIXME: Điền thông tin Gmail thật của bạn vào đây (và không push lên git)
    // Email gốc để gửi:
    private const string SmtpUser = "your.email@gmail.com"; 
    
    // Mật khẩu ứng dụng (App Password) 16 ký tự của Google, KHÔNG phải mật khẩu đăng nhập thông thường:
    private const string SmtpPass = "xxxx xxxx xxxx xxxx"; 

    private const string SmtpHost = "smtp.gmail.com";
    private const int SmtpPort = 587;

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Hệ thống CSVC PTIT", SmtpUser));
            message.To.Add(new MailboxAddress(toEmail, toEmail));
            message.Subject = subject;

            message.Body = new TextPart("plain")
            {
                Text = body
            };

            using var client = new SmtpClient();
            // Connect to SMTP using STARTTLS
            await client.ConnectAsync(SmtpHost, SmtpPort, SecureSocketOptions.StartTls);
            
            // Authenticate
            await client.AuthenticateAsync(SmtpUser, SmtpPass);
            
            // Send
            await client.SendAsync(message);
            
            // Disconnect
            await client.DisconnectAsync(true);
            
            System.Diagnostics.Debug.WriteLine($"[EMAIL SENT TO {toEmail}] Subject: {subject}");
        }
        catch (Exception ex)
        {
            // Log error
            System.Diagnostics.Debug.WriteLine($"[EMAIL FAILED] To: {toEmail}, Error: {ex.Message}");
            // Tuỳ thuộc vào nghiệp vụ, bạn có thể quăng lỗi lên trên hoặc nuốt lỗi
            throw new Exception($"Không thể gửi email đến {toEmail}. Chi tiết: {ex.Message}");
        }
    }
}
