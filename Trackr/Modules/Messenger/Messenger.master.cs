using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackrModels;
using Trackr.Utils;

namespace Trackr.Modules.Messenger
{
    public partial class Messenger : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void lnkCompose_Click(object sender, EventArgs e)
        {
            divCompose.Visible = true;
            ScriptManager.RegisterStartupScript(divCompose, divCompose.GetType(), "ToggleCompose", "$('.compose-message').modal('toggle');", true);
        }

        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {

        }

        protected void btnSendMessage_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsValid)
                {
                    return;
                }

                if (Page as Trackr.Page == null)
                {
                    throw new ArgumentException("Page is not inheriting Trackr.Page");
                }

                int clubID = (Page as Trackr.Page).CurrentUser.ClubID;

                List<string> emailRecipients = txtRecipients.Text.Split(',').Select(i => i.Trim().ToUpper()).Distinct().ToList();

                using (TrackrModels.UserManagement um = new TrackrModels.UserManagement())
                {
                    List<int> recipients = um.WebUsers.Where(i => i.ClubID == clubID && emailRecipients.Contains(i.Email.ToUpper())).Select(i => i.UserID).Distinct().ToList();

                    Message message = new Message()
                    {
                        Body = txtMessage.Text,
                        CreateDate = DateTime.UtcNow,
                        FromID = int.Parse(Page.User.Identity.Name),
                    };

                    recipients.ForEach(i =>
                    {
                        message.MessageRecipients.Add(new MessageRecipient()
                        {
                            UserID = i,
                            SentDate = DateTime.UtcNow
                        });
                    });

                    um.Add(message);
                    um.SaveChanges();

                    txtRecipients.Text = "";
                    txtMessage.Text = "";


                    //Master.AddAlert("Successfully sent message.", UI.AlertBoxType.Success);
                    divCompose.Visible = false;
                    upNested.Update();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "modal", "$('.modal-backdrop').remove()", true);
                }
            }
            catch (Exception ex)
            {
                Master.HandleException(ex);
            }
        }

        protected void validatorRecipientListOK_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (Page as Trackr.Page == null)
                {
                    throw new ArgumentException("Page is not inheriting Trackr.Page");
                }

                int clubID = (Page as Trackr.Page).CurrentUser.ClubID;

                List<string> emailRecipients = txtRecipients.Text.Split(',').Select(i => i.Trim().ToUpper()).Distinct().ToList();

                using (TrackrModels.UserManagement um = new TrackrModels.UserManagement())
                {
                    List<string> recipientsNotAuthorizedFor = um.WebUsers.Where(i => i.ClubID != clubID && emailRecipients.Contains(i.Email.ToUpper())).Select(i => i.Email).Distinct().ToList();

                    validatorRecipientListOK.ErrorMessage = string.Format(validatorRecipientListOK.ErrorMessage, string.Join(", ", recipientsNotAuthorizedFor));
                    args.IsValid = recipientsNotAuthorizedFor.Count() == 0;
                }
            }
            catch (Exception ex)
            {
                Master.HandleException(ex);
                args.IsValid = false;
            }
        }
    }
}