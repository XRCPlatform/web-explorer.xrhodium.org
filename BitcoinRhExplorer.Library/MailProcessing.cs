using System;
using System.Configuration;
using System.Net.Mail;

namespace BitcoinRhExplorer.Library
{
    public static class MailProcessing
    {
        public static void Send(string message, string subject, string targetEmail)
        {
            MailMessage m = new MailMessage();
            SmtpClient sc = new SmtpClient();

            m.From = new MailAddress(ConfigurationManager.AppSettings["mailFrom"]);
            m.To.Add(targetEmail);
            m.Subject = subject;
            m.Body = message;
            m.IsBodyHtml = true;
            sc.Host = ConfigurationManager.AppSettings["mailSmtp"];

            try
            {
                sc.Port = 25;
                sc.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["mailFrom"], ConfigurationManager.AppSettings["mailPsw"]);
                sc.EnableSsl = false;
                sc.Send(m);
            }
            catch (Exception ex)
            {
                throw new BitcoinRhExplorerException(ex.Message);
            }
        }
    }
}
