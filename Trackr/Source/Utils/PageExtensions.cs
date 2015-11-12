using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrackrModels;

namespace Trackr.Utils
{
    public static class PageExtensions
    {
        public static WebUser GetCurrentWebUser()
        {
            int userID;

            if (!HttpContext.Current.User.Identity.IsAuthenticated || !int.TryParse(HttpContext.Current.User.Identity.Name, out userID))
            {
                throw new Exception("User not authenticated");
            }

            WebUser user = HttpContext.Current.Session["CurrentWebUser"] as WebUser;

            if (user != null && user.UserID == userID)
            {
                return user;
            }
            else
            {
                using (WebUsersController wuc = new WebUsersController())
                {
                    HttpContext.Current.Session["CurrentWebUser"] = wuc.GetWhere(i => i.UserID == userID).First();
                    return HttpContext.Current.Session["CurrentWebUser"] as WebUser;
                }
            }
        }

    }
}