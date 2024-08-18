using MimeKit;

namespace Papyrus.Docs.Email.Service.Models
{
    public class Message(IEnumerable<EmailAddress> to, string subject, string content)
    {
        public List<MailboxAddress> To { get; set; } = [.. to.Select(x => new MailboxAddress(x.Name, x.Address))];
        public string Subject { get; set; } = subject;
        public string Content { get; set; } = content;
    }
}
