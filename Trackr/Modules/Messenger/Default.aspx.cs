using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Trackr.Modules.Messenger
{
    public partial class Default : Page
    {
        private class MessageDTO
        {
            public string Message { get; set; }
            public string From { get; set; }
            public int? FromID { get; set; }
            public DateTime SentDate { get; set; }
            public bool IsStarred { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            UpdateInboxMessages();
        }

        private int ResultsPerPage
        {
            get { return 50; }
        }

        private int PageIndex
        {
            get { return ViewState["Page"] as int? ?? 1; }
            set { ViewState["Page"] = value; }
        }

        private void UpdateInboxMessages()
        {
            List<MessageDTO> messages;
            using (TrackrModels.UserManagement um = new TrackrModels.UserManagement())
            {
                int userID = CurrentUser.UserID;
                messages = um.MessageRecipients.Where(i => i.UserID == userID).Select(i => new MessageDTO()
                {
                    Message = i.Message.Body,
                    IsStarred = i.IsStarred,
                    FromID = i.Message.FromID,
                    SentDate = i.SentDate
                }).OrderByDescending(i => i.SentDate).Skip(ResultsPerPage * (PageIndex - 1)).Take(ResultsPerPage).ToList();

                rptEmail.DataSource = messages;
            }

            rptEmail.DataBind();

            litResultsPPMin.Text = ((PageIndex - 1) * ResultsPerPage + 1).ToString();
            litResultsPPMax.Text = ((PageIndex - 1) * ResultsPerPage + ResultsPerPage).ToString();
            litResultsPPTotal.Text = messages.Count().ToString();
            divResultsPP.Visible = messages.Count() > 0;
            divNoMessages.Visible = messages.Count() == 0;
        }

        public System.Collections.IEnumerable rptEmail_GetData()
        {
            return null;
        }

    }
}