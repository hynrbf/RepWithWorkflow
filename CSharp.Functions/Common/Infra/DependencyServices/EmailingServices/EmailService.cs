using System.Diagnostics;
using Aspose.Email;
using Aspose.Email.Clients.Smtp;
using Aspose.Email.Mime;

namespace Common.Infra
{
    public class EmailService : IEmailService
    {
        // TODO. to move to vault soon.
        private const string Password = "3276T3ch";

        public bool SendEmail(string email, string subject, string body, Stream attachment, string attachmentFileName)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return false;
                }

                // TODO. as of now, we are using 'admin@dsforgs.com' email.
                // In order to use 'tech.dev@suntech.gi' email, we need to setup Authenticated SMTP
                // in Microsoft 365 Admin center.
                // ref. https://morgantechspace.com/2021/01/the-smtp-server-requires-a-secure-connection-or-the-client.html
                var fromEmail = Environment.GetEnvironmentVariable("SmtpEmail", EnvironmentVariableTarget.Process);

                if (string.IsNullOrEmpty(fromEmail))
                {
                    throw new Exception("An error was in EmailService.SendEmailAsync. Sender email is not set!");
                }

                using var smtpClient = new SmtpClient("smtp.office365.com", 587, fromEmail, Password);
                var mail = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mail.To.Add(email);

                if (attachment != null)
                {
                    mail.Attachments.Add(new Attachment(attachment, attachmentFileName));
                }

                mail.AlternateViews.Add(
                    AlternateView.CreateAlternateViewFromString(body, new ContentType("text/html"))
                );

                smtpClient.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error was in EmailService.SendEmailAsync with message {ex.Message}");
                throw;
            }
        }

        public bool SendEmail(string email, string subject, string body)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(body))
                {
                    throw new NullReferenceException(
                        $"Email to, subject and body should not be null in {nameof(EmailService)}.{nameof(SendEmail)}");
                }

                var fromEmail = Environment.GetEnvironmentVariable("SmtpEmail", EnvironmentVariableTarget.Process);

                if (string.IsNullOrEmpty(fromEmail))
                {
                    throw new Exception("An error was in EmailService.SendEmailAsync. Sender email is not set!");
                }

                using var smtpClient = new SmtpClient("smtp.office365.com", 587, fromEmail, Password);
                var mail = new MailMessage();
                mail.From = new MailAddress(fromEmail);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                mail.To.Add(email);
                mail.AlternateViews.Add(
                    AlternateView.CreateAlternateViewFromString(body, new ContentType("text/html"))
                );
                smtpClient.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error was in EmailService.SendEmailAsync with message {ex.Message}");
                throw;
            }
        }
    }
}