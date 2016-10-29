using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Trackr.Account
{
    public partial class ManagePassword : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }
        }

        protected void lnkSavePassword_Click(object sender, EventArgs e)
        {
            if (Membership.GetUser(CurrentUser.UserID).ChangePassword(txtCurrentPassword.Text, txtPassword.Text))
            {
                AlertBox.AddAlert("Successfully changed your password.");
            }
            else
            {
                AlertBox.AddAlert("Unable to change your password. Please make sure your current password is correct.", false, UI.AlertBoxType.Error);
            }
        }
    }
}