using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrackrModels;
using Trackr.Utils;
using Microsoft.AspNet.FriendlyUrls.Resolvers;

namespace Trackr
{
    public class Page : System.Web.UI.Page
    {
        public WebUser CurrentUser
        {
            get
            {
                return PageExtensions.GetCurrentWebUser();
            }
        }

        public void CheckAllowed(string permission)
        {
            using (WebUsersController wuc = new WebUsersController())
            {
                if (!wuc.IsAllowed(CurrentUser.UserID, permission))
                {
                    // optionally we can just make them go back to original page?
                    Response.Redirect("~/Default.aspx", true);
                }
            }
        }

        public void CheckAllowed<T, K>(string permission, K resourceID)
            where K : struct
            where T : IScopableController<K>, IDisposable, new()
        {
            using (T c = new T())
            {
                if (!c.IsUserScoped(CurrentUser.UserID, permission, resourceID))
                {
                    Response.Redirect("~/Default.aspx", true);
                }
            }
        }

        protected void Page_PreInit(object sender, System.EventArgs e)
        {
            // Check to make sure the user id in session is the same as the current one. If it is not, then we need to wipe out data so we don't show this user the old user's data
            int? sessionUserID = Session["__UserIDOnFile"] as int?;

            if (HttpContext.Current.User.Identity.IsAuthenticated && sessionUserID.HasValue && CurrentUser != null)
            {
                if (sessionUserID != CurrentUser.UserID)
                {
                    Session.Clear();
                    Session["__UserIDOnFile"] = CurrentUser.UserID;
                }
            }
            else
            {
                Session.Clear();

                if (HttpContext.Current.User.Identity.IsAuthenticated && CurrentUser != null)
                {
                    Session["__UserIDOnFile"] = CurrentUser.UserID;
                }
            }

            if (WebFormsFriendlyUrlResolver.IsMobileView(new HttpContextWrapper(Context)))
            {
                MasterPageFile = "~/Site.Mobile.Master";
            }
        }

        public void HandleException(Exception ex)
        {
            Master.HandleException(ex);
        }
    }
}