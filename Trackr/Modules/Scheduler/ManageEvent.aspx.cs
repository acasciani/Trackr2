using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.OpenAccess.FetchOptimization;
using TrackrModels;

namespace Trackr.Modules.Scheduler
{
    public partial class ManageEvent : Page
    {
        private int? TeamScheduleID
        {
            get
            {
                int _try;
                return int.TryParse(Request.QueryString["id"], out _try) ? _try : (int?)null;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            if (TeamScheduleID.HasValue)
            {
                //edit
                using (TeamSchedulesController tsc = new TeamSchedulesController())
                {
                    if (!tsc.IsUserScoped(CurrentUser.UserID, "Scheduler.EditEvent", TeamScheduleID.Value))
                    {
                        AlertBox_General.SetStatus("You do not have permission to edit this event.", UI.AlertBoxType.Error);
                        divManage.Visible = false;
                        return;
                    }
                    else
                    {
                        divManage.Visible = true;
                        Populate(TeamScheduleID.Value);
                    }
                }

                lnkUpdateEvent.Visible = true;
            }
            else
            {
                CheckAllowed("Scheduler.AddNewEvent");
                divManage.Visible = true;
                lnkCreateEvent.Visible = true;
                Title = "Add New Event";
            }
        }

        private void Populate(int teamScheduleID)
        {
            using (TeamSchedulesController tsc = new TeamSchedulesController())
            {
                FetchStrategy fetch = new FetchStrategy();
                fetch.LoadWith<TeamSchedule>(i => i.Team);

                TeamSchedule teamSchedule = tsc.GetWhere(i => i.TeamScheduleID == TeamScheduleID.Value, fetch).First();
                txtStartDate.Text = teamSchedule.StartDate.ToLocalTime().ToString("yyyy-MM-ddTHH:mm");
                txtEndDate.Text = teamSchedule.EndDate.ToLocalTime().ToString("yyyy-MM-ddTHH:mm");
                txtRSVPBy.Text = teamSchedule.RSVPBy.HasValue ? teamSchedule.RSVPBy.Value.ToLocalTime().ToString("yyyy-MM-ddTHH:mm") : null;
                txtEventName.Text = teamSchedule.EventName;
                chkEnableRSVP.Checked = teamSchedule.RSVPBy.HasValue;
                chkEnableRSVP_CheckedChanged(null, null);

                // set program/team
                ptpPicker.SelectedProgramID = teamSchedule.Team.ProgramID;
                ptpPicker.SelectedTeamID = teamSchedule.TeamID;
                ptpPicker.Populate();
            }
        }

        protected void lnkCreateEvent_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            try
            {
                using (TeamSchedulesController tsc = new TeamSchedulesController())
                {
                    tsc.AddNew(new TeamSchedule()
                    {
                        CreateDate = DateTime.Now.ToUniversalTime(),
                        CreateUserID = CurrentUser.UserID,
                        EventName = txtEventName.Text,
                        TeamID = ptpPicker.SelectedTeamID.Value,
                        StartDate = DateTime.Parse(txtStartDate.Text).ToUniversalTime(),
                        EndDate = DateTime.Parse(txtEndDate.Text).ToUniversalTime(),
                        RSVPBy = chkEnableRSVP.Checked && !string.IsNullOrWhiteSpace(txtRSVPBy.Text) ? DateTime.Parse(txtRSVPBy.Text).ToUniversalTime() : (DateTime?)null,
                        IsActive = true
                    });

                    Response.Redirect(ResolveUrl("~/Modules/Scheduler/"));
                }
            }
            catch (Exception ex)
            {
                AlertBox_Manage.SetStatus("An error occurred while trying to create new event.", UI.AlertBoxType.Error);
            }
        }

        protected void lnkUpdateEvent_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            try
            {
                using (TeamSchedulesController tsc = new TeamSchedulesController())
                {
                    // get clean copy
                    TeamSchedule teamSchedule = tsc.Get(TeamScheduleID.Value);
                    teamSchedule.ModifyDate = DateTime.Now.ToUniversalTime();
                    teamSchedule.ModifyUserID = CurrentUser.UserID;
                    teamSchedule.EventName = txtEventName.Text;
                    teamSchedule.TeamID = ptpPicker.SelectedTeamID.Value;
                    teamSchedule.StartDate = DateTime.Parse(txtStartDate.Text).ToUniversalTime();
                    teamSchedule.EndDate = DateTime.Parse(txtEndDate.Text).ToUniversalTime();
                    teamSchedule.RSVPBy = chkEnableRSVP.Checked && !string.IsNullOrWhiteSpace(txtRSVPBy.Text) ? DateTime.Parse(txtRSVPBy.Text).ToUniversalTime() : (DateTime?)null;

                    tsc.Update(teamSchedule);

                    Response.Redirect(ResolveUrl("~/Modules/Scheduler/"));
                }
            }
            catch (Exception ex)
            {
                AlertBox_Manage.SetStatus("An error occurred while trying to create new event.", UI.AlertBoxType.Error);
            }
        }


        #region validators
        protected void validatorStartDateParses_ServerValidate(object source, ServerValidateEventArgs args)
        {
            DateTime parse;
            args.IsValid = DateTime.TryParse(args.Value, out parse);
        }

        protected void validatorStartEndCompare_ServerValidate(object source, ServerValidateEventArgs args)
        {
            DateTime tryParseStart, tryParseEnd;
            if (DateTime.TryParse(txtStartDate.Text, out tryParseStart) && DateTime.TryParse(txtEndDate.Text, out tryParseEnd))
            {
                args.IsValid = tryParseEnd >= tryParseStart;
            }
            else
            {
                args.IsValid = false;
            }
        }

        protected void chkEnableRSVP_CheckedChanged(object sender, EventArgs e)
        {
            txtRSVPBy.Text = "";
            divRSVPBy.Visible = chkEnableRSVP.Checked;
        }
        #endregion
    }
}