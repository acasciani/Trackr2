using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Trackr.Utils
{
    public partial class Messenger
    {
        #region helper classes
        public enum EmailRecipientType{ TO, CC, BCC}

        [Serializable]
        public class EmailRecipient{
            public string Email{get;set;}
            public string Name{get;set;}
            public EmailRecipientType RecipientType{get;set;}
        }

        [Serializable]
        public class TemplateVariable
        {
            public string VariableName { get; set; }
            public string VariableContent { get; set; }
        }

        public class MandrillOKResponses
        {
            public List<MandrillOKResponse> MandrillResponses { get; set; }
        }

        public class MandrillOKResponse
        {
            public string email { get; set; }
            public string status { get; set; }
            public string reject_reason { get; set; }
            public string _id { get; set; }
        }
        #endregion


        // email api info
        #region API config settings
        private static string _EmailAPI_Key = null;
        private static string EmailAPI_Key
        {
            get
            {
                if (_EmailAPI_Key == null)
                {
                    _EmailAPI_Key = ConfigurationManager.AppSettings["MandrillKey"];
                }

                return _EmailAPI_Key;
            }
        }

        private static string _EmailAPI_EndPoint = null;
        private static string EmailAPI_EndPoint
        {
            get
            {
                if (_EmailAPI_EndPoint == null)
                {
                    _EmailAPI_EndPoint = ConfigurationManager.AppSettings["MandrillEndpoint"];
                }

                return _EmailAPI_EndPoint;
            }
        }

        private static bool? _MessengerMode_IsProduction = null;
        private static bool MessengerMode_IsProduction
        {
            get
            {
                if (_MessengerMode_IsProduction == null)
                {
                    _MessengerMode_IsProduction = ConfigurationManager.AppSettings["MessengerMode"] == "Production";
                }

                return _MessengerMode_IsProduction ?? false;
            }
        }

        private static string _EmailAPI_MessengerDevEmail = null;
        private static string EmailAPI_MessengerDevEmail
        {
            get
            {
                if (_EmailAPI_MessengerDevEmail == null)
                {
                    _EmailAPI_MessengerDevEmail = ConfigurationManager.AppSettings["MessengerDevEmail"];
                }

                return _EmailAPI_MessengerDevEmail;
            }
        }

        private static string _EmailAPI_MessengerProdEmail = null;
        private static string EmailAPI_MessengerProdEmail
        {
            get
            {
                if (_EmailAPI_MessengerProdEmail == null)
                {
                    _EmailAPI_MessengerProdEmail = ConfigurationManager.AppSettings["MessengerProdEmail"];
                }

                return _EmailAPI_MessengerProdEmail;
            }
        }

        private static List<EmailRecipient> GetDevelopmentRecipients()
        {
            //get all as unique people
            string[] recipients = EmailAPI_MessengerDevEmail.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);

            List<EmailRecipient> emailRecipients = new List<EmailRecipient>();

            foreach (string recipient in recipients)
            {
                string[] data = recipient.Split('|');
                emailRecipients.Add(new EmailRecipient()
                {
                    Email = data[0],
                    Name = data[1],
                    RecipientType = data[2] == "cc" ? EmailRecipientType.CC : data[2] == "bcc" ? EmailRecipientType.BCC : EmailRecipientType.TO
                });
            }

            return emailRecipients;
        }

        private static List<EmailRecipient> GetAdditionalProductionRecipients()
        {
            //get all as unique people
            string[] recipients = EmailAPI_MessengerProdEmail.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);

            List<EmailRecipient> emailRecipients = new List<EmailRecipient>();

            foreach (string recipient in recipients)
            {
                string[] data = recipient.Split('|');
                emailRecipients.Add(new EmailRecipient()
                {
                    Email = data[0],
                    Name = data[1],
                    RecipientType = data[2] == "cc" ? EmailRecipientType.CC : data[2] == "bcc" ? EmailRecipientType.BCC : EmailRecipientType.TO
                });
            }

            return emailRecipients;
        }
        #endregion

        public static Guid? SendEmail(string TemplateName, List<TemplateVariable> TemplateVariables, List<TemplateVariable> GlobalMergeVariables, List<EmailRecipient> ToRecipients, bool TrackOpens, bool TrackClicks)
        {
            try
            {
                RestClient client = new RestClient(EmailAPI_EndPoint);
                RestRequest request = new RestRequest("/messages/send-template.json", Method.POST);

                request.RequestFormat = DataFormat.Json;

                if (!MessengerMode_IsProduction)
                {
                    ToRecipients = GetDevelopmentRecipients();
                }
                else
                {
                    ToRecipients.AddRange(GetAdditionalProductionRecipients());
                }

                var messageStruct = new
                {
                    to = ToRecipients.Select(i => new
                    {
                        email = i.Email,
                        name = i.Name,
                        type = i.RecipientType == EmailRecipientType.BCC ? "bcc" : i.RecipientType == EmailRecipientType.CC ? "cc" : "to"
                    }).ToList(),

                    track_opens = TrackOpens,
                    track_clicks = TrackClicks,
                    merge_language = "handlebars",
                    global_merge_vars = (GlobalMergeVariables ?? Enumerable.Empty<TemplateVariable>()).Select(i => new { name = i.VariableName, content = i.VariableContent }).ToList()
                };

                var body = new
                {
                    key = EmailAPI_Key,
                    template_name = TemplateName,
                    template_content = (TemplateVariables ?? Enumerable.Empty<TemplateVariable>()).Select(i => new { name = i.VariableName, content = i.VariableContent }).ToList(),
                    message = messageStruct
                };

                request.AddBody(body);

                //execute
                IRestResponse<MandrillOKResponses> response = client.Execute<MandrillOKResponses>(request);

                if (response == null)
                {
                    var content = response.Content; // raw content as string
                    throw new Exception("An error occurred sending mail: " + content);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return ex.HandleException();
            }
        }
    }
}