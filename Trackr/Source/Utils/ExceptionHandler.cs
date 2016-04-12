using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Trackr
{
    public enum ExceptionPriority { Urgent = 0, High, Medium, Low }

    public static class ExceptionHandler
    {
        public static Guid HandleException(this Exception refr, ExceptionPriority priority = ExceptionPriority.Low)
        {
            Guid guid = Guid.NewGuid();
            EmailException(refr, priority, guid);
            return guid; // return a tracking number for user to contact support with
        }

        private static void EmailException(Exception ex, ExceptionPriority priority, Guid guid)
        {
            try
            {
                MailPriority mailpriority = MailPriority.Normal;
                string To = "";
                switch (priority)
                {
                    case ExceptionPriority.Urgent:
                        To = "coachcasciani+EH_Urgent@gmail.com";
                        mailpriority = MailPriority.High;
                        break;
                    case ExceptionPriority.High:
                        To = "coachcasciani+EH_High@gmail.com";
                        mailpriority = MailPriority.High;
                        break;
                    case ExceptionPriority.Medium:
                        To = "coachcasciani+EH_Mid@gmail.com";
                        break;
                    default:
                        To = "coachcasciani+EH_Low@gmail.com";
                        mailpriority = MailPriority.Low;
                        break;
                }

                string Subject = string.Format("{0} - My Law Tools Exception", guid);

                string body = "";

                string PageURL = HttpContext.Current.Request.Url.AbsolutePath;
                string Referrer = HttpContext.Current.Request.UrlReferrer == null ? "null" : HttpContext.Current.Request.UrlReferrer.AbsolutePath;
                string UserAgent = HttpContext.Current.Request.UserAgent;
                string UserHostAddress = HttpContext.Current.Request.UserHostAddress;
                string User = HttpContext.Current.User.Identity.Name;

                body = string.Format("Page URL: {0} \r\nReferrer URL: {1} \r\nUser Agent: {2} \r\nUser Host Address: {3} \r\nUser: {4} \r\nDate: {5}\r\nTracking #: {6} \r\nPriority: {7} \r\n\n\n",
                    PageURL, Referrer, UserAgent, UserHostAddress, User, DateTime.Now, guid, priority);
                body += string.Format("Message: {0}\r\n\n\n", ex.Message);
                body += string.Format("Stack Trace: {0}\r\n\n\n", ex.StackTrace);
                body += string.Format("Inner Exception: {0}\r\n\n\n", ex.InnerException);
                body += string.Format("Exception Source: {0}\r\n\n\n", ex.Source);
                body += "--- This message was auto-generated ---";

                using (SmtpClient client = new SmtpClient())
                {
                    MailMessage message = new MailMessage();
                    message.To.Add(new MailAddress(To));
                    message.Subject = Subject;
                    message.Body = body;
                    message.Priority = mailpriority;
                    message.From = new MailAddress("coachcasciani@gmail.com");
                    client.Send(message);
                }
            }
            catch (Exception ex2)
            {
                // gracefully fail
                throw new Exception("second expcetion fail");
            }
        }
    }
}