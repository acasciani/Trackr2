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

                using (WebUsersController wuc = new WebUsersController())
                using(NewUserMappingsController numc = new NewUserMappingsController())
                using(ScopeAssignmentsController sac = new ScopeAssignmentsController())
                {
                    WebUser webUser = wuc.Get((int)user.ProviderUserKey);
                    webUser.ClubID = 1;
                    wuc.Update(webUser);

                    // map the new user to roles
                    var roleIDs = numc.GetWhere(i => i.ClubID == webUser.ClubID).Select(i => i.RoleID).Distinct().ToList();

                    foreach (int roleID in roleIDs)
                    {
                        ScopeAssignment assignment = new ScopeAssignment()
                        {
                            IsDeny = false,
                            ScopeID = 5,
                            UserID = UserID.Value,
                            ResourceID = UserID.Value,
                            RoleID = roleID
                        };

                        sac.AddNew(assignment);
                    }
                }
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

            string redirect = FormsAuthentication.GetRedirectUrl(user.ProviderUserKey.ToString(), true);

            dhlContinue.NavigateUrl = redirect;
            pFinish.Visible = !string.IsNullOrWhiteSpace(redirect);
            pRedirect.Visible = string.IsNullOrWhiteSpace(redirect);
            
        }
    }
}