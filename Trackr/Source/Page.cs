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

        public void CheckAllowed<T,K,Q>(string permission, Q resourceID) where T : IScopable<K, Q>, IDisposable, new()
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
            if (WebFormsFriendlyUrlResolver.IsMobileView(new HttpContextWrapper(Context)))
            {
                MasterPageFile = "~/Site.Mobile.Master";
            }
        }
    }
}