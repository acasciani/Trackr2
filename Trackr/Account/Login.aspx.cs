using System;
using System.Web;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using System.Web.Security;
using TrackrModels;

namespace Trackr.Account
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterHyperLink.NavigateUrl = "Register";
            // Enable this once you have account confirmation enabled for password reset functionality
            //ForgotPasswordHyperLink.NavigateUrl = "Forgot";
            var returnUrl = HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
            if (!String.IsNullOrEmpty(returnUrl))
            {
                RegisterHyperLink.NavigateUrl += "?ReturnUrl=" + returnUrl;
            }
        }

        protected void LogIn(object sender, EventArgs e)
        {
            if (IsValid)
            {
                using (WebUsersController wuc = new WebUsersController())
                {
                    if (Membership.ValidateUser(Email.Text, Password.Text))
                    {
                        FormsAuthentication.RedirectFromLoginPage(Membership.GetUser(Email.Text).ProviderUserKey.ToString(), RememberMe.Checked);
                    }
                    else
                    {
                        AlertBox.SetStatus("The email address and password entered do not match.", UI.AlertBoxType.Error);
                    }
                }
            }
        }
    }
}