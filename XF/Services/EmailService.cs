using System.IO;
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
            var isSsl = ConfigService.GetValue("EmailSSL",_context).ToLower() == "true";
            SmtpClient smtp = new SmtpClient(host, port);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(user, password);
            smtp.EnableSsl = isSsl;
            smtp.Send(message);
        }

        private MailMessage GetSimpleEmailMessage(string emailToSend,string subject,string body,Stream stream,string fileName)
        {
            var message = new MailMessage();
            var emailFrom = ConfigService.GetValue("EmailFrom", _context);
            message.From = new MailAddress( emailFrom);
            message.To.Add(new MailAddress(emailToSend));
            message.IsBodyHtml = true;
            message.Subject = subject;
            message.Body = body;
            if (stream!=null)
            {
                message.Attachments.Add(new Attachment(stream, fileName));
            }
            return message;
        }

        public void SendInvoiceToClient(Invoice invoice,Client client,Stream stream,string fileName)
        {
            var subjectTemplate = invoice.InvoiceStatusId == 1 ?
            ConfigService.GetValue("EmailSubjectTemplate", _context).Replace("Invoice","Estimate")
            : ConfigService.GetValue("EmailSubjectTemplate", _context);

            subjectTemplate = string.Format(subjectTemplate, invoice.Id);
            var emailToSend = client.Email;
            var emailTemplate = Resources.InvoiceEmailTemplate.ToString()
                .Replace("{{ClientName}}",client.FullName)
                .Replace("{{InvoiceId}}", invoice.Id.ToString());
            var message = GetSimpleEmailMessage(emailToSend, subjectTemplate, emailTemplate,stream,fileName);
            SendEmail(message);
        }

    }
}