using JabbR.Infrastructure;
using System.Net.Mail;
using System.Net.Mime;

namespace JabbR.Services
{
    public class SmtpClientEmailSender : IEmailSender
    {
        public void Send(Email email)
        {
            if (email == null)
            {
                throw new System.ArgumentNullException("email");
            }

            using (var message = CreateMailMessage(email))
            {
                using (var client = new SmtpClient())
                {
                    client.Send(message);
                }
            }
        }

        private static MailMessage CreateMailMessage(Email email)
        {
            var message = new MailMessage { From = new MailAddress(email.From), Subject = email.Subject };

            if (!string.IsNullOrEmpty(email.Sender))
            {
                message.Sender = new MailAddress(email.Sender);
            }

            email.To.Each(to => message.To.Add(to));
            email.ReplyTo.Each(to => message.ReplyToList.Add(to));
            email.CC.Each(cc => message.CC.Add(cc));
            email.Bcc.Each(bcc => message.Bcc.Add(bcc));
            email.Headers.Each(pair => message.Headers[pair.Key] = pair.Value);

            if (!string.IsNullOrEmpty(email.HtmlBody) && !string.IsNullOrEmpty(email.TextBody))
            {
                message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(email.HtmlBody, new ContentType(ContentTypes.Html)));
                message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(email.TextBody, new ContentType(ContentTypes.Text)));
            }
            else if (!string.IsNullOrEmpty(email.TextBody))
            {
                message.Body = email.TextBody;
                message.IsBodyHtml = false;
            }
            else if (!string.IsNullOrEmpty(email.HtmlBody))
            {
                message.Body = email.HtmlBody;
                message.IsBodyHtml = true;
            }

            return message;
        }
    }
}