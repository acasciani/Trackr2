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
            if (!IsAllowed(permission))
            {
                // optionally we can just make them go back to original page?
                Response.Redirect("~/Default.aspx", true);
            }
        }

        public bool IsAllowed(string permission)
        {
            using (WebUsersController wuc = new WebUsersController())
            {
                return wuc.IsAllowed(CurrentUser.UserID, permission);
            }
        }


        public void CheckAllowed<T, K>(K resourceID, bool useOrLogic, params string[] permissions)
            where K : struct
            where T : IScopableController<K>, IDisposable, new()
        {
            using (T c = new T())
            {
                int counter = 0;

                foreach (string permission in permissions)
                {
                    if (c.IsUserScoped(CurrentUser.UserID, permission, resourceID))
                    {
                        counter += 1;
                    }
                }

                if (counter == 0)
                {
                    // passes neither and nor or logic
                    Response.Redirect("~/Default.aspx", true);
                }
                else
                {
                    if (counter < permissions.Length && !useOrLogic)
                    {
                        //and logic
                        Response.Redirect("~/Default.aspx", true);
                    }

                    // or logic passed if we get here
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