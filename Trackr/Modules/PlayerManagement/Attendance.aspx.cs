using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Trackr.Modules.PlayerManagement
{
    public partial class Attendance : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                int teamScheduleID;
                if (int.TryParse(Request.Params.Get("__EVENTARGUMENT"), out teamScheduleID))
                {
                    widgetAttendanceTracking.Visible = true;
                    widgetAttendanceTracking.TeamScheduleID = teamScheduleID;
                }
            }
        }
    }
}