using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Trackr.Modules.Scheduler
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                int teamScheduleID;
                if (Page.FindControl(Request.Params.Get("__EVENTTARGET")) == updatePanel && int.TryParse(Request.Params.Get("__EVENTARGUMENT"), out teamScheduleID))
                {
                    divWidgetContainer.Visible = true;
                    widgetAttendanceTracking.TeamScheduleID = teamScheduleID;
                    lnkManageEvent.NavigateUrl = string.Format(lnkManageEvent.NavigateUrl, teamScheduleID);
                }
            }
        }
    }
}