using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Trackr.WebHandlers.Messenger
{
    /// <summary>
    /// Summary description for Recipients
    /// </summary>
    public class Recipients : IHttpHandler
    {
        [Serializable]
        public class DTO
        {
            public DateTime LastCorrespondance { get; set; }
            public string PersonName { get; set; }
            public string Email { get; set; }
        }


        public void ProcessRequest(HttpContext context)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                context.Response.StatusCode = 401;
                return;
            }

            int currentUserID = int.Parse(context.User.Identity.Name);

            context.Response.ContentType = "application/json";

            JsonSerializer serializer = JsonSerializer.Create();
            List<DTO> results = null;

            string query = context.Request.QueryString["query"];

            if (string.IsNullOrWhiteSpace(query))
            {
                results = GetMostRecent(currentUserID);
            }
            else
            {
                results = GetBySearch(currentUserID, query);
            }

            using (JsonWriter writer = new JsonTextWriter(context.Response.Output))
            {
                serializer.Serialize(writer, results);
            }
        }

        private List<DTO> GetBySearch(int currentUserID, string query)
        {
            using(TrackrModels.UserManagement um = new TrackrModels.UserManagement())
            using (MessengerRecipientsController mrc = new MessengerRecipientsController())
            {
                // get users this person recently sent to
                return mrc.GetMatchingEmailRecipients(query, um.WebUsers.Single(i => i.UserID == currentUserID).ClubID).Select(i => new DTO()
                {
                    Email = i.Email,
                    LastCorrespondance = DateTime.MinValue,
                    PersonName = i.FirstName + " " + i.LastName
                }).ToList();
            }
        }

        private List<DTO> GetMostRecent(int currentUserID)
        {
            Dictionary<int, DateTime> MostRecentCorrespondance = null;

            using (TrackrModels.UserManagement um = new TrackrModels.UserManagement())
            {
                // get users this person recently sent to
                MostRecentCorrespondance = um.MessageRecipients.Where(i => i.Message.FromID == currentUserID).Select(i => new
                {
                    UserID = i.UserID,
                    CorrespondenceDate = i.SentDate
                }).GroupBy(i => i.UserID).Select(i => new
                {
                    UserID = i.Key,
                    LatestCorrespondenceDate = i.Max(j => j.CorrespondenceDate)
                }).OrderByDescending(i => i.LatestCorrespondenceDate).Take(20).ToDictionary(i => i.UserID, i => i.LatestCorrespondenceDate);
            }

            if (MostRecentCorrespondance == null)
            {
                return null;
            }

            List<int> userIDs = MostRecentCorrespondance.Keys.ToList();
            Dictionary<int, string> UserNames = null;

            using (TrackrModels.ClubManagement cm = new TrackrModels.ClubManagement())
            {
                UserNames = cm.People.Where(i => i.UserID.HasValue && userIDs.Contains(i.UserID.Value)).Select(i => new
                {
                    UserID = i.UserID.Value,
                    Name = i.FName + " " + i.LName
                }).ToDictionary(i => i.UserID, i => i.Name);
            }

            Dictionary<int, string> Emails = null;
            using (TrackrModels.UserManagement um = new TrackrModels.UserManagement())
            {
                Emails = um.WebUsers.Where(i => userIDs.Contains(i.UserID)).Select(i => new
                {
                    UserID = i.UserID,
                    Email = i.Email
                }).ToDictionary(i => i.UserID, i => i.Email);
            }

            var data = userIDs.Select(i => new DTO()
            {
                LastCorrespondance = MostRecentCorrespondance[i],
                PersonName = UserNames[i],
                Email = Emails[i]
            }).OrderByDescending(i => i.LastCorrespondance).ToList();

            return data;
        }



        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}