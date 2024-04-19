namespace Common
{
    public interface IEmailService
    {
        bool SendEmail(string email, string subject, string body, Stream attachment, string attachmentFileName);
        bool SendEmail(string email, string subject, string body);
    }
}
