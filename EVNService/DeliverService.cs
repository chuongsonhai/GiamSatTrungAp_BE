using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using log4net;

namespace EVNService
{
    public class DeliverService : IDeliverService
    {
        private static ILog log = LogManager.GetLogger(typeof(DeliverService));

        public void SendMail(IList<SendMail> mails)
        {
            string host = ConfigurationManager.AppSettings["MAIL_HOST"];
            string port = ConfigurationManager.AppSettings["MAIL_PORT"];
            string mailFrom = ConfigurationManager.AppSettings["MAIL_FROM"];
            string mailSender = ConfigurationManager.AppSettings["MAIL_SENDER"];
            string mailPassword = ConfigurationManager.AppSettings["MAIL_PASS"];
            foreach (SendMail mail in mails)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(mail.EMAIL) || !mail.EMAIL.Contains("@"))
                    {
                        Utils.UpdateMailStatus(mail.ID, 1);
                        continue;
                    }
                    MailMessage mailMessage = new MailMessage(mailFrom, mail.EMAIL, mail.TIEUDE, mail.NOIDUNG);
                    mailMessage.BodyEncoding = Encoding.UTF8;
                    mailMessage.From = new MailAddress(mailFrom, mailFrom);
                    mailMessage.IsBodyHtml = true;
                    SmtpClient smtpClient = new SmtpClient(host, int.Parse(port));
                    if (!string.IsNullOrEmpty(mailSender) && !string.IsNullOrEmpty(mailPassword))
                    {
                        smtpClient.Credentials = new NetworkCredential(mailSender, mailPassword);
                    }                    
                    smtpClient.ServicePoint.MaxIdleTime = 1;
                    smtpClient.EnableSsl = true;
                    smtpClient.Send(mailMessage);
                    Utils.UpdateMailStatus(mail.ID, 1);
                }
                catch (Exception message)
                {
                    log.Error(message);
                    Utils.UpdateMailStatus(mail.ID, 2);
                }
            }
        }               
    }
}
