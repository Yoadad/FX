using System.Net;
using System.Net.Mail;
using XF.Entities;

namespace XF.Services
{
    public class EmailService
    {
        public static void SendEmail(MailMessage message, XFModel context)
        {
            var host = ConfigService.GetValue("EmailHost",context);
            var port = int.Parse(ConfigService.GetValue("EmailPort",context));
            var user = ConfigService.GetValue("EmailUser", context);
            var password = ConfigService.GetValue("EmailPassword", context);

            SmtpClient smtp = new SmtpClient(host, port);
            smtp.EnableSsl = false;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(user, password);
            smtp.EnableSsl = false;
            smtp.Send(message);
        }

        public MailMessage GetSimpleEmailMessage(XFModel context,string emailToSend,string subject,string body)
        {
            var message = new MailMessage();
            var emailFrom = ConfigService.GetValue("EmailFrom", context);
            message.From = new MailAddress( emailFrom);
            message.To.Add(new MailAddress(emailToSend));
            message.IsBodyHtml = true;
            message.Subject = subject;
            message.Body = body;
            return message;
        }
    }
}