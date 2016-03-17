using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.OpenAccess.FetchOptimization;
using TrackrModels;

namespace Trackr.Source.Controls
{
    public partial class AttendanceTrackingWidget : UserControl
    {
        public int TeamScheduleID { get; set; }

        public string EventName { get; set; }
        public string TeamName { get; set; }
        public DateTime Starts { get; set; }
        public DateTime Ends { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptInclude(Page, Page.GetType(), "AttendanceTracking", "/Scripts/AttendanceTracking.js");

//            ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "test", "$('.attendance-ticker').click(function(){MarkPresent();});", true);

            using (TeamSchedulesController tsc = new TeamSchedulesController())
            {
                FetchStrategy fetch = new FetchStrategy() { MaxFetchDepth = 5 };
                fetch.LoadWith<TeamSchedule>(i => i.Team);
                fetch.LoadWith<Team>(i => i.TeamPlayers);
                fetch.LoadWith<TeamPlayer>(i => i.Player);
                fetch.LoadWith<Player>(i => i.Person);
                
                TeamSchedule schedule = tsc.GetWhere(i => i.TeamScheduleID == TeamScheduleID, fetch).First();

                EventName = schedule.EventName;
                TeamName = schedule.Team.TeamName;
                Starts = schedule.StartDate;
                Ends = schedule.EndDate;

                rptPlayer.DataSource = schedule.Team.TeamPlayers;
                rptPlayer.DataBind();
            }
        }

        protected void lnk_Click(object sender, EventArgs e)
        {
            (sender as LinkButton).Visible = false;
        }
    }
}