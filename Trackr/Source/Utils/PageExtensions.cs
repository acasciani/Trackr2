using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Trackr.Controls;
using Trackr.UI;
using TrackrModels;

namespace Trackr.Utils
{
    public static class PageExtensions
    {
        public static void HandleException(this MasterPage master, Exception ex)
        {
            Guid trackingNumber = ex.HandleException();

            MasterPage rootMaster = master;

            while (rootMaster.Master != null)
            {
                rootMaster = rootMaster.Master;
            }

            AlertBox ExceptionAlerts = rootMaster.FindControl("ExceptionAlerts") as AlertBox;

            if (ExceptionAlerts != null)
            {
                ExceptionAlerts.AddAlert(string.Format("An error occurred while processing your request. The tracking number is: {0}", trackingNumber.ToString()), false, UI.AlertBoxType.Error);
            }
        }

        public static WebUser GetCurrentWebUser()
        {
            int userID;

           
            if (!HttpContext.Current.User.Identity.IsAuthenticated || !int.TryParse(HttpContext.Current.User.Identity.Name, out userID))
            {
                //throw new Exception("User not authenticated");
                return null;
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