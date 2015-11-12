using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using TrackrModels;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace Trackr.Account
{
    public partial class Register : Page
    {
        private int? UserID
        {
            get { return ViewState["UserID"] as int?; }
            set { ViewState["UserID"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AlertBox.HideStatus();
        }

        protected void CreateWizard_CreatedUser(object sender, EventArgs e)
        {
            MembershipUser user = Membership.GetUser(((CreateUserWizard)sender).UserName);
            if (user != null)
            {
                UserID = (int)user.ProviderUserKey;
            }
        }

        protected void Step2_Personal_Deactivate(object sender, EventArgs e)
        {
            using (WebUserInfosController wuic = new WebUserInfosController())
            {
                // add new info
                WebUserInfo info = new WebUserInfo()
                {
                    FName = txtFirstName.Text,
                    MInitial = string.IsNullOrWhiteSpace(txtMiddleInitial.Text) ? (char?)null : (char)txtMiddleInitial.Text.ToCharArray(0, 1)[0],
                    LName = txtLastName.Text,
                    UserID = UserID.Value
                };

                wuic.AddNew(info);
            }
        }

        protected void CreateWizard_CreateUserError(object sender, CreateUserErrorEventArgs e)
        {
            string plainEnglishMsg = "An error occurred while creating your account.";

            switch (e.CreateUserError)
            {
                case MembershipCreateStatus.DuplicateEmail: plainEnglishMsg = "This email address is already in use and cannot be used again."; break;
                case MembershipCreateStatus.DuplicateProviderUserKey: plainEnglishMsg = "The provider key specified is already in use and cannot be used again."; break;
                case MembershipCreateStatus.DuplicateUserName: plainEnglishMsg = "This email address is already in use and cannot be used again."; break;
                case MembershipCreateStatus.InvalidEmail: plainEnglishMsg = "This is an invalid email address."; break;
                default: break;
            }

            AlertBox.SetStatus(plainEnglishMsg, UI.AlertBoxType.Error);
        }

        protected void validatorEmailExists_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = Membership.FindUsersByName(args.Value).Count == 0;
        }

        protected void Step3_Completed_Activate(object sender, EventArgs e)
        {
            MembershipUser user = Membership.GetUser(CreateWizard.UserName);
            FormsAuthentication.SetAuthCookie(user.ProviderUserKey.ToString(), true);
        }
    }
}