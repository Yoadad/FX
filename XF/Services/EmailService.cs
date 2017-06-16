using System.Net;
using System.Net.Mail;
using XF.Entities;
using XF.Properties;

namespace XF.Services
{
    public class SendEmailService
    {
        private XFModel _context;
        public SendEmailService(XFModel context)
        {
            _context = context;
        }
        private  void SendEmail(MailMessage message)
        {
            var host = ConfigService.GetValue("EmailHost",_context);
            var port = int.Parse(ConfigService.GetValue("EmailPort",_context));
            var user = ConfigService.GetValue("EmailUser", _context);
            var password = ConfigService.GetValue("EmailPassword", _context);

            SmtpClient smtp = new SmtpClient(host, port);
            smtp.Timeout = 100000;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(user, password);
            smtp.EnableSsl = false;
            smtp.Send(message);
        }

        private MailMessage GetSimpleEmailMessage(string emailToSend,string subject,string body)
        {
            var message = new MailMessage();
            var emailFrom = ConfigService.GetValue("EmailFrom", _context);
            message.From = new MailAddress( emailFrom);
            message.To.Add(new MailAddress(emailToSend));
            message.IsBodyHtml = true;
            message.Subject = subject;
            message.Body = body;
            return message;
        }

        public void SendInvoiceToClient(Client client)
        {
            var subjectTemplate = ConfigService.GetValue("EmailSubjectTemplate", _context);
            var emailToSend = client.Email;
            var emailTemplate = Resources.InvoiceEmailTemplate.ToString();
            var message = GetSimpleEmailMessage(emailToSend, subjectTemplate, emailTemplate);
            SendEmail(message);
        }

    }
}