using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Trackr.Source.Controls;

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
                    //divWidgetContainer.Visible = true;
                    widgetAttendanceTracking.TeamScheduleID = teamScheduleID;
                    divWidget.Visible = true;
                }
            }
        }

        protected void widgetAttendanceTracking_TeamScheduleDeleted(object sender, EventArgs e)
        {
            AttendanceTrackingWidget.TeamScheduleEvent tse = (AttendanceTrackingWidget.TeamScheduleEvent)e;

            if (tse.MessageType == UI.AlertBoxType.Success)
            {
                widgetAttendanceTracking.TeamScheduleID = null;
                divWidget.Visible = false;
            }

            AlertBox.SetStatus(tse.Message, tse.MessageType);
        }
    }
}