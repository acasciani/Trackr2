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
        private class PlayerAttendance
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public bool Present { get; set; }
            public int PlayerID { get; set; }
        }

        public int? TeamScheduleID
        {
            get { return ViewState["TeamScheduleID"] as int?; }
            set { ViewState["TeamScheduleID"] = value; }
        }

        public string EventName { get; set; }
        public string TeamName { get; set; }
        public DateTime Starts { get; set; }
        public DateTime Ends { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!TeamScheduleID.HasValue)
            {
                return;
            }

            using (TeamSchedulesController tsc = new TeamSchedulesController())
            {
                FetchStrategy fetch = new FetchStrategy() { MaxFetchDepth = 5 };
                fetch.LoadWith<TeamSchedule>(i => i.Team);
                fetch.LoadWith<Team>(i => i.TeamPlayers);
                fetch.LoadWith<TeamPlayer>(i => i.Player);
                fetch.LoadWith<Player>(i => i.Person);
                fetch.LoadWith<TeamSchedule>(i => i.Attendances);

                TeamSchedule schedule = tsc.GetWhere(i => i.TeamScheduleID == TeamScheduleID.Value, fetch).First();

                EventName = schedule.EventName;
                TeamName = schedule.Team.TeamName;
                Starts = schedule.StartDate;
                Ends = schedule.EndDate;

                var knownAttendances = schedule.Attendances.ToList();

                var data = schedule.Team.TeamPlayers.Select(i => new PlayerAttendance()
                {
                    FirstName = i.Player.Person.FName,
                    LastName = i.Player.Person.LName,
                    Present = knownAttendances.Where(j => j.PlayerID == i.PlayerID).Count() > 0,
                    PlayerID = i.Player.PlayerID
                });

                rptPlayer.DataSource = data;
                rptPlayer.DataBind();
            }
        }

        protected void btnPlayer_Click(object sender, EventArgs e)
        {
            using (AttendancesController ac = new AttendancesController())
            {
                ClickablePanel panel = (ClickablePanel)sender;

                ac.AddNew(new Attendance()
                {
                    CreateDate = DateTime.Now.ToUniversalTime(),
                    CreateUserID = CurrentUser.UserID,
                    IsActive = true,
                    PlayerID = panel.PlayerID,
                    TeamScheduleID = TeamScheduleID.Value
                });

                panel.CssClass += " success";
            }
        }
    }
}